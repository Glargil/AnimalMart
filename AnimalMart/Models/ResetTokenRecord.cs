namespace AnimalMart.Models
{
    // Read-only projection of a PasswordResetTokens row - not a full entity,
    // just what the service layer needs to validate/consume a token.
    public record ResetTokenRecord(int Id, int UserId, DateTime ExpiresAtUtc);
}
