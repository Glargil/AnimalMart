using Isopoh.Cryptography.Argon2;

namespace AnimalMart.Security;
public static class Argon2PasswordHasher
{
    public static string HashPassword(string password)
    {
        return Argon2.Hash(password);
    }

    public static bool VerifyPassword(string password, string encodedHash)
    {
        return Argon2.Verify(encodedHash, password);
    }
}