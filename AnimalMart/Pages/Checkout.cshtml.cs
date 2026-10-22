using AnimalMart.Factories;
using AnimalMart.Models;
using AnimalMart.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AnimalMart.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly IShoppingCartService _cartService;
        private readonly PaymentStrategyFactory _strategyFactory;
        private readonly CheckoutService _checkoutService;

        public CheckoutModel(
            IShoppingCartService cartService,
            PaymentStrategyFactory strategyFactory,
            CheckoutService checkoutService)
        {
            _cartService = cartService;
            _strategyFactory = strategyFactory;
            _checkoutService = checkoutService;
        }

        public ShoppingCart Cart { get; set; } = new();
        public string? Result { get; set; }

        public void OnGet()
        {
            Cart = _cartService.GetCart();
        }

        public IActionResult OnPost(PaymentMethod paymentMethod)
        {
            // Enum binding: anything but Card/MobilePay fails model binding
            // and is rejected here before any payment logic runs.
            if (!ModelState.IsValid)
            {
                Cart = _cartService.GetCart();
                return Page();
            }

            Cart = _cartService.GetCart();

            _checkoutService.SetStrategy(_strategyFactory.Create(paymentMethod));

            try
            {
                Result = _checkoutService.Checkout(Cart);
                _cartService.ClearCart();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }

            return Page();
        }
    }
}
