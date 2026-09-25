using AnimalMart.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AnimalMart.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly IPasswordResetService _resetService;
        private readonly IPasswordAnalyzer _analyzer;

        public ResetPasswordModel(IPasswordResetService resetService, IPasswordAnalyzer analyzer)
        {
            _resetService = resetService;
            _analyzer = analyzer;
        }

        // Bound from the query string on GET ("?token=...") and carried
        // forward as a hidden field on the POST form.
        [BindProperty(SupportsGet = true)]
        public string Token { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool LinkInvalid { get; private set; }
        public bool Success { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Only checks validity for display purposes - does NOT consume
            // the token. See PasswordResetService.ValidateTokenAsync for why
            // (corporate mail scanners "prefetch" links before the real
            // user clicks them; consuming on GET would kill their link).
            bool isValid = await _resetService.ValidateTokenAsync(Token);
            LinkInvalid = !isValid;
            return Page();
        }

        // Same live strength-meter handler as CreateUserModel.OnGetScore,
        // reused here so the reset page shows the identical score/feedback
        // as sign-up while the user types. No username is available on this
        // page without resolving the token to a user first, so that one
        // check in the analyzer (password containing the username) is
        // skipped here, same as in OnPostAsync below.
        public IActionResult OnGetScore([FromQuery] string? password)
        {
            var result = _analyzer.Analyze(password ?? "", null);
            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Same policy used at registration (PasswordAnalyzerService),
            // so a reset can't produce a weaker password than sign-up allows.
            var analysis = _analyzer.Analyze(NewPassword, null);
            if (!analysis.MeetsPolicy)
            {
                foreach (var message in analysis.Feedback)
                {
                    ModelState.AddModelError(nameof(NewPassword), message);
                }
            }

            if (!ModelState.IsValid)
            {
                // Re-check so a validation failure doesn't show the form for
                // an already-dead link.
                LinkInvalid = !await _resetService.ValidateTokenAsync(Token);
                return Page();
            }

            bool success = await _resetService.ResetPasswordAsync(Token, NewPassword);
            if (!success)
            {
                LinkInvalid = true;
                return Page();
            }

            Success = true;
            return Page();
        }
    }
}
