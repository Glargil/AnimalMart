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

        public void OnGet()
        {
            Animals = _animalRepo.GetAll().ToList();
            Items = _itemRepo.GetAll().ToList();
        }
    }
}