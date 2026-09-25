using Microsoft.AspNetCore.Mvc.RazorPages;
using AnimalMart.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace AnimalMart.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IUserService _userService;

        public LoginModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public UserLoginDTO Credentials {get; set;} = new();

        public string? ErrorMessage {get; set;}

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }
            
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var attempt = _userService.ValidateLogin(Credentials, ip);

            switch (attempt.Result)
            {
                case LoginResult.LockedOut:
                    ErrorMessage = "Too many failed login attempts. Please try again i a few minutes";
                    return Page();

                case LoginResult.InvalidCredentials:
                    ErrorMessage = "Invalid email or password";
                    return Page();

                case LoginResult.Success:
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, attempt.User!.Id.ToString()),
                        new Claim(ClaimTypes.Name, attempt.User.Name),
                        new Claim(ClaimTypes.Email, attempt.User.Email),
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    return RedirectToPage("Index");

                default:
                    return Page();
            }
        }
    }
}
