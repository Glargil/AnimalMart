using AnimalMart.Interfaces;
using Npgsql;

namespace AnimalMart.Repos
{
    public class UserRepo : IUserRepo
    {
        private readonly string _connectionString;

        public UserRepo(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found."
                );
        }

        public User Create(User user)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                const string query =
                    @"
                    INSERT INTO users (name, email, phone_number, password_hash)
                    VALUES (@name, @email, @phone_number, @password_hash)
                    RETURNING id";

                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", user.Name);
                    command.Parameters.AddWithValue("@email", user.Email);
                    command.Parameters.AddWithValue(
                        "@phone_number",
                        (object?)user.PhoneNumber ?? DBNull.Value
                    );
                    command.Parameters.AddWithValue("@password_hash", user.PasswordHash);

                    var newId = command.ExecuteScalar();
                    user.Id = Convert.ToInt32(newId);
                }
            }
            return user;
        }

        public void Delete(int userId)
        {
            throw new NotImplementedException();
        }

        public List<User> GetAll()
        {
            var users = new List<User>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                const string query =
                    "SELECT id, name, email, phone_number, password_hash FROM users";
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
                                ? null
                                : reader.GetString(reader.GetOrdinal("phone_number")),
                            PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
                        };
                        users.Add(user);
                    }
                }
            }
            return users;
        }

        public User GetById(int userId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                const string query =
                    "SELECT id, name, email, phone_number, password_hash FROM users WHERE id = @id";
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            // Matches the rest of this class's not-found handling:
                            // GetByEmail returns null for "not found", but the IUserRepo
                            // signature for GetById is non-nullable, so a caller passing
                            // a stale/unknown id gets an explicit failure instead of a
                            // silent null that would NullReferenceException later.
                            throw new KeyNotFoundException($"No user found with id {userId}.");
                        }
                        return new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            PhoneNumber = reader.IsDBNull(reader.GetOrdinal("phone_number"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("phone_number")),
                            PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
                        };
                    }
                }
            }
        }

        public User? GetByEmail(string email)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                const string query =
                    "SELECT id, name, email, phone_number, password_hash FROM users WHERE lower(email) = lower(@email)";
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }
                        return new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            PhoneNumber = reader.IsDBNull(reader.GetOrdinal("phone_number"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("phone_number")),
                            PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
                        };
                    }
                }
            }
        }

        public User Update(User user)
        {
            throw new NotImplementedException();
        }

        public async Task UpdatePasswordHashAsync(int userId, string newPasswordHash)
        {
            const string sql = "UPDATE users SET password_hash = @password_hash WHERE id = @id;";

            await using var conn = new NpgsqlConnection(_connectionString);
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@password_hash", newPasswordHash);
            cmd.Parameters.AddWithValue("@id", userId);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
