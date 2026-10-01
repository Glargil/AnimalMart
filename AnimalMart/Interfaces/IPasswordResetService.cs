namespace AnimalMart.Interfaces
{
    public interface IPasswordResetService
    {
            Task RequestPasswordResetAsync(string email, string baseUrl);
            Task<bool> ValidateTokenAsync(string token);
            Task<bool> ResetPasswordAsync(string token, string newPassword);
    }
}
