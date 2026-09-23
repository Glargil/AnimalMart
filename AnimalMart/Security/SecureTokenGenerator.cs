using System.Security.Cryptography;
using System.Text;

namespace AnimalMart.Security
{
    /// <summary>
    /// Generates and hashes one-time password-reset tokens.
    ///
    /// The raw token is 256 bits (32 bytes) from a CSPRNG (RandomNumberGenerator,
    /// not System.Random) — this is what goes in the email link and is never
    /// stored anywhere. Only SHA-256(token) is persisted.
    ///
    /// Why SHA-256 here but Argon2 for passwords? Argon2 exists to slow down
    /// attackers guessing a *low-entropy, human-chosen* secret. This token has
    /// 256 bits of CSPRNG entropy — guessing it is computationally infeasible
    /// regardless of hash speed — so a fast hash is the right tool: it keeps
    /// every reset-link click cheap, and if the DB leaks, the hash alone gives
    /// an attacker nothing they can use to reconstruct the token.
    /// </summary>
    public static class SecureTokenGenerator
    {
        private const int TokenByteLength = 32; // 256 bits

        public static string GenerateToken()
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(TokenByteLength);
            return Base64UrlEncode(bytes);
        }

        public static byte[] HashToken(string token)
        {
            return SHA256.HashData(Encoding.UTF8.GetBytes(token));
        }

        // URL-safe base64 (no '+', '/', or '=' padding) so the token drops
        // cleanly into a query string without extra escaping surprises.
        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }

}
