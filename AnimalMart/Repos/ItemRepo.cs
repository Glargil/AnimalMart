using Npgsql;

namespace AnimalMart.Repos
{
    public class ItemRepo : IItemRepo
    {
        // Joins the base table with the subtype table so one row has every Item field.
        private const string SelectSql = @"
            SELECT p.id, p.price, p.name, p.description, i.stock
            FROM products p
            JOIN items i ON i.id = p.id";

        private readonly string _connectionString;
        private readonly ItemFactory _factory;

        public ItemRepo(IConfiguration configuration, ItemFactory factory)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found."
                );
            _factory = factory;
        }

        public IEnumerable<Item> GetAll()
        {
            var items = new List<Item>();
            using var connection = new NpgsqlConnection(_connectionString);
            using var command = new NpgsqlCommand(SelectSql + " ORDER BY p.id", connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add(_factory.Create(reader));
            }
            return items;
        }

        public Item? GetById(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            using var command = new NpgsqlCommand(SelectSql + " WHERE p.id = @id", connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? _factory.Create(reader) : null;
        }

        public Item Add(Item item)
        {
            // Both inserts run as ONE statement, so they succeed or fail together:
            // the CTE inserts into products and hands its new id to the items insert.
            const string sql = @"
                WITH new_product AS (
                    INSERT INTO products (price, name, description)
                    VALUES (@price, @name, @description)
                    RETURNING id
                )
                INSERT INTO items (id, stock)
                SELECT id, @stock FROM new_product
                RETURNING id";

            using var connection = new NpgsqlConnection(_connectionString);
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@price", item.Price);
            command.Parameters.AddWithValue("@name", item.Name!);
            command.Parameters.AddWithValue("@description", item.Description!);
            command.Parameters.AddWithValue("@stock", item.Stock);

            connection.Open();
            item.Id = Convert.ToInt32(command.ExecuteScalar());
            return item;
        }
    }
}