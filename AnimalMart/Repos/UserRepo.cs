using Npgsql;
using System.Data;

namespace AnimalMart.Repos
{
    public class UserRepo : IUserRepo
    {
        private readonly string _connectionString;

        public UserRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public User CreateUser(User user)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                //const string query = @"
                //INSERT INTO users (name, email, phone_number, password_hash)
                //OUTPUT INSERTED.Id
                //VALUES (@Name, @Email, @PhoneNumber, @PasswordHash)";
                const string query = @"
                    INSERT INTO users (name, email, phone_number, password_hash)
                    VALUES (@name, @email, @phone_number, @password_hash)
                    RETURNING id";

                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", user.Name);
                    command.Parameters.AddWithValue("@email", user.Email);
                    command.Parameters.AddWithValue("@phone_number", (object?)user.PhoneNumber 
                        ?? DBNull.Value);
                    command.Parameters.AddWithValue("@password_hash", user.PasswordHash);

                    var newId = command.ExecuteScalar();
                    user.Id = Convert.ToInt32(newId);
                    
                }
            }
            return user;
        }

        public void DeleteUser(int userId)
        {
            throw new NotImplementedException();
        }
        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                const string query = "SELECT id, name, email, phone_number, password_hash FROM users";
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //IsDBNull checks are used to handle potential null values in the database for optional fields like PhoneNumber and PasswordHash.
                        var user = new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            PhoneNumber = reader.IsDBNull(reader.GetOrdinal("phone_number"))
                                ? null : reader.GetString(reader.GetOrdinal("phone_number")),
                            PasswordHash = reader.GetString(reader.GetOrdinal("password_hash"))
                        };
                        users.Add(user);
                    }
                }
            }
            return users;
        }

        public User GetUser(int userId)
        {
            throw new NotImplementedException();
        }

        public User UpdateUser(User user)
        {
            throw new NotImplementedException();
        }
        
    }
}
