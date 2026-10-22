using AnimalMart.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AnimalMart.Pages
{
    public class CartIndexModel : PageModel
    {
        private readonly IShoppingCartService _cartService;

        public CartIndexModel(IShoppingCartService cartService)
        {
            _cartService = cartService;
        }

        public ShoppingCart Cart { get; set; } = new();

        public void OnGet()
        {
            Cart = _cartService.GetCart();
        }

        public IActionResult OnPostUpdateQuantity(int productId, ProductType productType, int quantity)
        {
            _cartService.UpdateQuantity(productId, productType, quantity);
            return RedirectToPage();
        }

        public IActionResult OnPostRemove(int productId, ProductType productType)
        {
            _cartService.RemoveProduct(productId, productType);
            return RedirectToPage();
        }

        public IActionResult OnPostClear()
        {
            _cartService.ClearCart();
            return RedirectToPage();
        }
    }
}
