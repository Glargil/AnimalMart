using AnimalMart.Factories;
using AnimalMart.Models;
using AnimalMart.Services;
using AnimalMart.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace AnimalMart.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly IShoppingCartService _cartService;
        private readonly PaymentStrategyFactory _strategyFactory;
        private readonly CheckoutService _checkoutService;
        private readonly IOrderRepo _orderRepo;

        public CheckoutModel(
            IShoppingCartService cartService,
            PaymentStrategyFactory strategyFactory,
            CheckoutService checkoutService,
            IOrderRepo orderRepo)
        {
            _cartService = cartService;
            _strategyFactory = strategyFactory;
            _checkoutService = checkoutService;
            _orderRepo = orderRepo;
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
        // After a successful payment the cart is cleared, so this is the
        // only place (and time) the completed order can be recorded for
        // the order history page. Name and unit price are snapshotted
        // from the cart: the history must show what was actually paid,
        // even if the product is later deleted or re-priced.
        // Guest checkouts (no logged-in user) are not recorded — there
        // is no user to show the history to.
        private void SaveOrderHistory(PaymentMethod paymentMethod)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdValue, out var userId))
            {
                return;
            }

            var order = new Order
            {
                UserId = userId,
                PaymentMethod = paymentMethod.ToString(),
                Total = Cart.TotalPrice,
                Lines = Cart.Lines
                    .Select(line => new OrderLine
                    {
                        ProductId = line.Product.Id,
                        ProductName = line.Product.Name ?? string.Empty,
                        UnitPrice = line.Product.Price,
                        Quantity = line.Quantity,
                        LineTotal = line.Subtotal,
                    })
                    .ToList(),
            };

            _orderRepo.Create(order);
        }
    }
}
