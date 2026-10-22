using AnimalMart.Factories;
using AnimalMart.Models;
using AnimalMart.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AnimalMart.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly IItemRepo _itemRepo;
        private readonly PaymentStrategyFactory _strategyFactory;
        private readonly CheckoutService _checkoutService;

        public CheckoutModel(
            IItemRepo itemRepo,
            PaymentStrategyFactory strategyFactory,
            CheckoutService checkoutService)
        {
            _itemRepo = itemRepo;
            _strategyFactory = strategyFactory;
            _checkoutService = checkoutService;
        }

        public ShoppingCart Cart { get; set; } = new();
        public string? Result { get; set; }

        public void OnGet()
        {
            LoadDemoCart();
        }

        public IActionResult OnPost(PaymentMethod paymentMethod)
        {
            // Enum binding: anything but Card/MobilePay fails model binding
            // and is rejected here before any payment logic runs.
            if (!ModelState.IsValid)
            {
                LoadDemoCart();
                return Page();
            }

            LoadDemoCart();

            _checkoutService.SetStrategy(_strategyFactory.Create(paymentMethod));

            try
            {
                Result = _checkoutService.Checkout(Cart);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }

            return Page();
        }

        // POC only: the "add to cart" story is not implemented yet,
        // so the cart is filled with the first two products in the
        // database to make checkout testable. Replace with the real
        // cart once that story lands.
        private void LoadDemoCart()
        {
            Cart = new ShoppingCart();
            foreach (var item in _itemRepo.GetAll().Take(2))
            {
                Cart.AddProduct(item);
            }
        }
    }
}