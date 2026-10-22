using AnimalMart.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AnimalMart.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IAnimalRepo _animalRepo;
        private readonly IItemRepo _itemRepo;

        public IndexModel(IAnimalRepo animalRepo, IItemRepo itemRepo)
        {
            _animalRepo = animalRepo;
            _itemRepo = itemRepo;
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
    }
}