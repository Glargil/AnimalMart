using AnimalMart.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace AnimalMart.Pages
{
    [EnableRateLimiting("PasswordResetPolicy")]
    public class ForgotPasswordModel : PageModel
    {
        // Padding responses to at least this long stops an attacker from using
        // response time to tell "email exists" (DB write + email send) apart
        // from "email doesn't exist" (near-instant return).
        private static readonly TimeSpan MinimumResponseTime = TimeSpan.FromMilliseconds(500);

        private readonly IPasswordResetService _resetService;
        private readonly ILogger<ForgotPasswordModel> _logger;

        public ForgotPasswordModel(IPasswordResetService resetService, ILogger<ForgotPasswordModel> logger)
        {
            _resetService = resetService;
            _logger = logger;
        }

        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public bool Submitted { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Built from server-controlled request info, never from user input,
            // and must resolve to https:// in any real deployment.
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var stopwatch = Stopwatch.StartNew();
            try
            {
                await _resetService.RequestPasswordResetAsync(Email, baseUrl);
            }
            catch (Exception ex)
            {
                // Never leak internal errors (SMTP down, DB hiccup, etc.) to the
                // client — that itself can leak information. Log and continue
                // to the same generic confirmation.
                _logger.LogError(ex, "Password reset request failed unexpectedly.");
            }

            var elapsed = stopwatch.Elapsed;
            if (elapsed < MinimumResponseTime)
            {
                await Task.Delay(MinimumResponseTime - elapsed);
            }

            // Always the same outcome regardless of whether the email existed,
            // was throttled, or genuinely triggered a reset.
            Submitted = true;
            return Page();
        }
    }
}
