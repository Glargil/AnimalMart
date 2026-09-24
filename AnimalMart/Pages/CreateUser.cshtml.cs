using AnimalMart.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AnimalMart.Pages
{
    public class CreateUserModel : PageModel
    {
        private readonly IPasswordAnalyzer _analyzer;
        [BindProperty]
        public string? Password { get; set; }
        private readonly IUserService _userService;

        public CreateUserModel(IUserService userService, IPasswordAnalyzer analyzer)
        {
            _userService = userService;
            _analyzer = analyzer;
        }

        [BindProperty]
        public UserCreateDTO NewUser { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }
        public IActionResult OnGetScore([FromQuery] string? password, [FromQuery] string? username)
        {
            var result = _analyzer.Analyze(password ?? "", username);
            return new JsonResult(result);
        }

        public IActionResult OnPost()
        {
            var analysis = _analyzer.Analyze(NewUser.Password ?? "", NewUser.Name);

            if (!analysis.MeetsPolicy)
                ModelState.AddModelError("NewUser.Password",
                "Passwordet overholder ikke password-policyn.");

            if (!ModelState.IsValid)
                return Page();

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