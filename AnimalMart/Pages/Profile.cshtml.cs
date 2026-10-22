using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace AnimalMart.Pages
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public void OnGet()
        {
            Name = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
            Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        }
        
        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToPage("Index");
        }
    }
}
