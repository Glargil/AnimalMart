using Microsoft.AspNetCore.Authorization;
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
    }
}
