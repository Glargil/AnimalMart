using AnimalMart.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AnimalMart.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IAnimalRepo _animalRepo;
        private readonly IItemRepo _itemRepo;
        private readonly IShoppingCartService _cartService;

        public IndexModel(IAnimalRepo animalRepo, IItemRepo itemRepo, IShoppingCartService cartService)
        {
            _animalRepo = animalRepo;
            _itemRepo = itemRepo;
            _cartService = cartService;
        }

        public List<Animal> Animals { get; set; } = new();
        public List<Item> Items { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public void OnGet()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                Animals = _animalRepo.GetAll().ToList();
                Items = _itemRepo.GetAll().ToList();
            }
            else
            {
                Animals = _animalRepo.Search(SearchTerm).ToList();
                Items = _itemRepo.Search(SearchTerm).ToList();
            }
        }

        public IActionResult OnPostAddToShoppingCart(int productId, ProductType productType)
        {
            _cartService.AddProduct(productId, productType);
            return RedirectToPage("./Index");
        }
    }
}
