using AnimalMart.Models;

namespace AnimalMart.Interfaces
{
    public interface IPasswordResetTokenRepo
    {
        Task<DateTime?> GetLastTokenCreatedAtAsync(int userId);
        Task InvalidateActiveTokensForUserAsync(int userId);
        Task CreateTokenAsync(int userId, byte[] tokenHash, DateTime expiresAtUtc);
        Task<ResetTokenRecord?> FindValidTokenAsync(byte[] tokenHash);
        Task DeleteTokenAsync(int tokenId);
        Task DeleteExpiredTokensAsync();
    }
}
