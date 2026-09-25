using System.Data;
using Npgsql;
using NpgsqlTypes;
using AnimalMart.Interfaces;
using AnimalMart.Models;

namespace AnimalMart.Repos
{
    public class PasswordResetTokenRepo : IPasswordResetTokenRepo
    {
        private readonly string _connectionString;

        public PasswordResetTokenRepo(IConfiguration configuration)
        {
            _connectionString =
                    configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                        "Connection string 'DefaultConnection' not found."
                    );
        }

        public async Task<DateTime?> GetLastTokenCreatedAtAsync(int userId)
        {
            const string sql = @"
                SELECT CreatedAtUtc
                FROM PasswordResetTokens
                WHERE UserId = @UserId
                ORDER BY CreatedAtUtc DESC
                LIMIT 1;";

            await using var conn = new NpgsqlConnection(_connectionString);
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add("@UserId", NpgsqlDbType.Integer).Value = userId;

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result is DateTime dt ? dt : null;
        }

        // Deletes ALL outstanding tokens for a user. Called when a new reset is
        // requested (so only the newest link is valid) and after a successful
        // reset (so no stale link can still be used).
        public async Task InvalidateActiveTokensForUserAsync(int userId)
        {
            const string sql = "DELETE FROM PasswordResetTokens WHERE UserId = @UserId;";

            await using var conn = new NpgsqlConnection(_connectionString);
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add("@UserId", NpgsqlDbType.Integer).Value = userId;

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task CreateTokenAsync(int userId, byte[] tokenHash, DateTime expiresAtUtc)
        {
            const string sql = @"
                INSERT INTO PasswordResetTokens (UserId, TokenHash, ExpiresAtUtc, CreatedAtUtc)
                VALUES (@UserId, @TokenHash, @ExpiresAtUtc, NOW());";

            await using var conn = new NpgsqlConnection(_connectionString);
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add("@UserId", NpgsqlDbType.Integer).Value = userId;
            cmd.Parameters.Add("@TokenHash", NpgsqlDbType.Bytea).Value = tokenHash;
            cmd.Parameters.Add("@ExpiresAtUtc", NpgsqlDbType.TimestampTz).Value = expiresAtUtc;

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<ResetTokenRecord?> FindValidTokenAsync(byte[] tokenHash)
        {
            const string sql = @"
                SELECT Id, UserId, ExpiresAtUtc
                FROM PasswordResetTokens
                WHERE TokenHash = @TokenHash;";

            await using var conn = new NpgsqlConnection(_connectionString);
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add("@TokenHash", NpgsqlDbType.Bytea).Value = tokenHash;

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new ResetTokenRecord(
                Id: reader.GetInt32(0),
                UserId: reader.GetInt32(1),
                ExpiresAtUtc: reader.GetDateTime(2));
        }

        public async Task DeleteTokenAsync(int tokenId)
        {
            const string sql = "DELETE FROM PasswordResetTokens WHERE Id = @Id;";

            await using var conn = new NpgsqlConnection(_connectionString);
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add("@Id", NpgsqlDbType.Integer).Value = tokenId;

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteExpiredTokensAsync()
        {
            const string sql = "DELETE FROM PasswordResetTokens WHERE ExpiresAtUtc < NOW();";

            await using var conn = new NpgsqlConnection(_connectionString);
            await using var cmd = new NpgsqlCommand(sql, conn);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
