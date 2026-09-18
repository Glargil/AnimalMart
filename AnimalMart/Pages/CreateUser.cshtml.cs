using AnimalMart.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AnimalMart.Pages
{
    public class CreateUserModel : PageModel
    {
        private readonly IUserService _userService;

        public CreateUserModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public UserCreateDTO NewUser { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _userService.CreateUser(NewUser);
                return RedirectToPage("Index"); // adjust to wherever you want to land after creation
            }
            catch (Exception)
            {
                ErrorMessage = "Something went wrong while creating the user. Please try again.";
                return Page();
            }
        }
    }
}