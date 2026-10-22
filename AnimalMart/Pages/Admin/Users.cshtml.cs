using AnimalMart.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace AnimalMart.Pages.Admin
{
    // Only a SuperAdmin reaches this page: Admins manage products,
    // SuperAdmins manage Admins.
    [Authorize(Roles = "SuperAdmin")]
    public class UsersModel : PageModel
    {
        private readonly IUserRepo _userRepo;

        public UsersModel(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        // The only roles assignable through this page. The repo
        // re-checks against its own allowlist as well.
        public List<string> AllRoles { get; } = new() { "Customer", "Admin", "SuperAdmin" };

        public List<User> Users { get; set; } = new();
        public int CurrentUserId { get; set; }
        public string? Error { get; set; }

        public void OnGet()
        {
            CurrentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            LoadUsers();
        }

        // Called by the small per-row form: hidden userId + role select.
        public IActionResult OnPostUpdateRole(int userId, string role)
        {
            CurrentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Self-demote guard: without this, one misclick could remove
            // the only SuperAdmin and lock the app out of user management.
            if (userId == CurrentUserId)
            {
                Error = "You cannot change your own role.";
                LoadUsers();
                return Page();
            }

            try
            {
                _userRepo.UpdateRole(userId, role);
                TempData["Success"] =
                    $"Role of user {userId} changed to {role}. They must log in again for it to take effect.";
            }
            catch (ArgumentException ex)
            {
                Error = ex.Message;
            }
            catch (KeyNotFoundException)
            {
                Error = "This user no longer exists.";
            }

            LoadUsers();
            return Page();
        }

        private void LoadUsers()
        {
            // User objects carry PasswordHash, but the page below never renders it.
            Users = _userRepo.GetAll();
        }
    }
}