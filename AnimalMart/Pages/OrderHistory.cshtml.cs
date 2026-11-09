using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using AnimalMart.Interfaces;

namespace AnimalMart.Pages
{
    [Authorize]
    public class OrderHistoryModel : PageModel
    {
        private readonly IOrderRepo _orderRepo;

        public OrderHistoryModel(IOrderRepo orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public List<Order> Orders { get; set; } = new();

        public void OnGet()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdValue, out var userId))
            {
                Orders = _orderRepo.GetByUserId(userId);
            }
        }
    }
}