using Npgsql;

namespace AnimalMart.Repos
{
    public class AnimalRepo : IAnimalRepo
    {
        // Joins the base table with the subtype table so one row has every Animal field.
        private const string SelectSql = @"
            SELECT p.id, p.price, p.name, p.description, p.img_path, a.sex, a.species, a.birthday
            FROM products p
            JOIN animals a ON a.id = p.id";

        private readonly string _connectionString;
        private readonly AnimalFactory _factory;

        public AnimalRepo(IConfiguration configuration, AnimalFactory factory)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found."
                );
            _factory = factory;
        }

        public IEnumerable<Animal> GetAll()
        {
            var animals = new List<Animal>();
            using var connection = new NpgsqlConnection(_connectionString);
            using var command = new NpgsqlCommand(SelectSql + " ORDER BY p.id", connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                animals.Add(_factory.Create(reader));
            }
            return animals;
        }

        public IEnumerable<Animal> Search(string term)
        {
            var animals = new List<Animal>();
            using var connection = new NpgsqlConnection(_connectionString);
            using var command = new NpgsqlCommand(
                SelectSql + " WHERE p.name ILIKE @term OR p.description ILIKE @term ORDER BY p.id",
                connection
            );
            command.Parameters.AddWithValue("@term", $"%{term}%");
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                animals.Add(_factory.Create(reader));
            }
            return animals;
        }

        public Animal? GetById(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            using var command = new NpgsqlCommand(SelectSql + " WHERE p.id = @id", connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? _factory.Create(reader) : null;
        }

        public Animal Add(Animal animal)
        {
            // Both inserts run as ONE statement, so they succeed or fail together:
            // the CTE inserts into products and hands its new id to the animals insert.
            const string sql = @"
                WITH new_product AS (
                    INSERT INTO products (price, name, description, img_path)
                    VALUES (@price, @name, @description, @img_path)
                    RETURNING id
                )
                INSERT INTO animals (id, sex, species, birthday)
                SELECT id, @sex, @species, @birthday FROM new_product
                RETURNING id";

            using var connection = new NpgsqlConnection(_connectionString);
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@price", animal.Price);
            command.Parameters.AddWithValue("@name", animal.Name!);
            command.Parameters.AddWithValue("@description", animal.Description!);
            command.Parameters.AddWithValue("@img_path", (object?)animal.ImgPath ?? DBNull.Value); // null -> SQL NULL
            command.Parameters.AddWithValue("@sex", animal.Sex!);
            command.Parameters.AddWithValue("@species", animal.Species!);
            command.Parameters.AddWithValue("@birthday", animal.BirthDay); // DateOnly maps to DATE

            connection.Open();
            animal.Id = Convert.ToInt32(command.ExecuteScalar());
            return animal;
        }
        public void Update(Animal animal)
        {
            // The base row and the subtype row must change together,
            // so both updates run inside one transaction.
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                const string productSql = @"
                    UPDATE products
                    SET price = @price, name = @name, description = @description
                    WHERE id = @id";

                int rows;
                using (var command = new NpgsqlCommand(productSql, connection, transaction))
                {
                    command.Parameters.AddWithValue("@price", animal.Price);
                    command.Parameters.AddWithValue("@name", animal.Name!);
                    command.Parameters.AddWithValue("@description", animal.Description!);
                    command.Parameters.AddWithValue("@id", animal.Id);
                    rows = command.ExecuteNonQuery();
                }

                if (rows == 0)
                {
                    // Nothing was updated: the id does not exist (anymore).
                    throw new KeyNotFoundException($"No animal found with id {animal.Id}.");
                }

                const string animalSql = @"
                    UPDATE animals
                    SET sex = @sex, species = @species, birthday = @birthday
                    WHERE id = @id";

                using (var command = new NpgsqlCommand(animalSql, connection, transaction))
                {
                    command.Parameters.AddWithValue("@sex", animal.Sex!);
                    command.Parameters.AddWithValue("@species", animal.Species!);
                    command.Parameters.AddWithValue("@birthday", animal.BirthDay);
                    command.Parameters.AddWithValue("@id", animal.Id);
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void Delete(int id)
        {
            // Deleting the products row cascades to the animals row
            // (and removes the product from all carts via cart_items).
            const string sql = "DELETE FROM products WHERE id = @id";

            using var connection = new NpgsqlConnection(_connectionString);
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}