using Npgsql;

namespace AnimalMart.Repos
{
    public class AnimalRepo : IAnimalRepo
    {
        // Joins the base table with the subtype table so one row has every Animal field.
        private const string SelectSql = @"
            SELECT p.id, p.price, p.name, p.description, a.sex, a.species, a.birthday
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
                    INSERT INTO products (price, name, description)
                    VALUES (@price, @name, @description)
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
            command.Parameters.AddWithValue("@sex", animal.Sex!);
            command.Parameters.AddWithValue("@species", animal.Species!);
            command.Parameters.AddWithValue("@birthday", animal.BirthDay); // DateOnly maps to DATE

            connection.Open();
            animal.Id = Convert.ToInt32(command.ExecuteScalar());
            return animal;
        }
    }
}