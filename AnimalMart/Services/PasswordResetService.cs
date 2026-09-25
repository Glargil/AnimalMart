using AnimalMart.Interfaces;
using AnimalMart.Security;

namespace AnimalMart.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private static readonly TimeSpan TokenLifetime = TimeSpan.FromMinutes(15);

        // Per-account throttle, independent of the IP-based rate limiting on the
        // endpoint (see Program.snippet.cs). Stops one account's inbox being
        // bombed by repeated submissions even from different IPs/proxies.
        private static readonly TimeSpan ResendCooldown = TimeSpan.FromSeconds(60);

        private readonly IUserRepo _userRepository;
        private readonly IPasswordResetTokenRepo _tokenRepository;
        private readonly IEmailSender _emailSender;

        public PasswordResetService(
            IUserRepo userRepository,
            IPasswordResetTokenRepo tokenRepository,
            IEmailSender emailSender)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _emailSender = emailSender;
        }

        public async Task RequestPasswordResetAsync(string email, string baseUrl)
        {
            // Normalize so "User@Example.com" and "user@example.com " hit the
            // same row and the same throttle bucket.
            email = email.Trim().ToLowerInvariant();

            var user = _userRepository.GetByEmail(email);

            // CRITICAL: never reveal whether the email exists. The Razor Page
            // shows the exact same "if your email exists..." message whether or
            // not this method does anything at all.
            if (user is null)
            {
                return;
            }

            var lastCreated = await _tokenRepository.GetLastTokenCreatedAtAsync(user.Id);
            if (lastCreated is not null && DateTime.UtcNow - lastCreated < ResendCooldown)
            {
                // Too soon since the last request for this account — silently
                // do nothing rather than send another email or error out.
                return;
            }

            // Only the newest link should ever work.
            await _tokenRepository.InvalidateActiveTokensForUserAsync(user.Id);

            string token = SecureTokenGenerator.GenerateToken();
            byte[] tokenHash = SecureTokenGenerator.HashToken(token);
            DateTime expiresAt = DateTime.UtcNow.Add(TokenLifetime);

            await _tokenRepository.CreateTokenAsync(user.Id, tokenHash, expiresAt);

            // baseUrl is built by the caller from Request.Scheme/Request.Host
            // (trusted, server-controlled values) — never from user input.
            // In production this must resolve to an https:// URL; see README.
            string resetLink = $"{baseUrl}/ResetPassword?token={Uri.EscapeDataString(token)}";

            string body =
                "We received a request to reset the password for your account.\n\n" +
                "Click the link below to choose a new password. This link expires in " +
                "15 minutes and can only be used once:\n\n" +
                $"{resetLink}\n\n" +
                "If you did not request this, you can safely ignore this email — " +
                "your password will not be changed.";

            // Deliberately no password (old or new) appears anywhere in this email.
            await _emailSender.SendAsync(user.Email, "Reset your password", body);
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            byte[] tokenHash = SecureTokenGenerator.HashToken(token);
            var record = await _tokenRepository.FindValidTokenAsync(tokenHash);

            // NOTE: this does NOT consume/delete the token. It's called from the
            // reset page's GET handler purely to decide what to render. Some
            // corporate mail security gateways "prefetch" links in emails to
            // scan them before the user ever clicks — if GET burned the token,
            // the real user's link would already be dead. The token is only
            // consumed in ResetPasswordAsync, on a successful POST.
            return record is not null && record.ExpiresAtUtc > DateTime.UtcNow;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            byte[] tokenHash = SecureTokenGenerator.HashToken(token);
            var record = await _tokenRepository.FindValidTokenAsync(tokenHash);

            if (record is null || record.ExpiresAtUtc <= DateTime.UtcNow)
            {
                return false;
            }

            string newHash = Argon2PasswordHasher.HashPassword(newPassword);
            await _userRepository.UpdatePasswordHashAsync(record.UserId, newHash);

            // Delete this token (single-use satisfied) and any other
            // outstanding tokens for the same user — a password change should
            // invalidate every other pending reset link too.
            await _tokenRepository.InvalidateActiveTokensForUserAsync(record.UserId);

            var user = _userRepository.GetById(record.UserId);
            if (user is not null)
            {
                await _emailSender.SendAsync(
                    user.Email,
                    "Your password was changed",
                    "Your password was just reset successfully.\n\n" +
                    "If this wasn't you, please contact support immediately — " +
                    "someone else may have access to your email or account.");
            }

            return true;
        }
    }
}