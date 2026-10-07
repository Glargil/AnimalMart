using System.Globalization;
using AnimalMart.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AnimalMart.Models;

namespace AnimalMart.Pages.Admin
{
    public class CreateProductModel : PageModel
    {
        private readonly AnimalFactory _animalFactory;
        private readonly ItemFactory _itemFactory;
        private readonly IAnimalRepo _animalRepo;
        private readonly IItemRepo _itemRepo;

        public CreateProductModel(
            AnimalFactory animalFactory,
            ItemFactory itemFactory,
            IAnimalRepo animalRepo,
            IItemRepo itemRepo
        )
        {
            _animalFactory = animalFactory;
            _itemFactory = itemFactory;
            _animalRepo = animalRepo;
            _itemRepo = itemRepo;
        }

        // Both forms bind here, but only the submitted form's fields are posted.
        // Everything is nullable so the other (empty) form doesn't trigger implicit [Required] errors.
        [BindProperty]
        public AnimalCreateDTO Animal { get; set; } = new();

        [BindProperty]
        public ItemCreateDTO Item { get; set; } = new();

        public string? AnimalError { get; set; }
        public string? ItemError { get; set; }

        // Shown below the forms so you can see the saved products right away.
        public List<Animal> Animals { get; set; } = new();
        public List<Item> Items { get; set; } = new();

        public void OnGet()
        {
            LoadProducts();
        }

        // Called by the form with asp-page-handler="AddAnimal".
        public IActionResult OnPostAddAnimal()
        {
            if (Animal.BirthDay is null || !TryParsePrice(Animal.Price, out var price))
            {
                AnimalError = "Birthday and a valid price are required.";
                LoadProducts();
                return Page();
            }

            try
            {
                var animal = _animalFactory.CreateNew(
                    Animal.Sex ?? "",
                    Animal.Species ?? "",
                    Animal.BirthDay.Value,
                    price,
                    Animal.Name ?? "",
                    Animal.Description ?? ""
                );
                _animalRepo.Add(animal);

                TempData["Success"] = $"Animal '{animal.Name}' saved with id {animal.Id}.";
                return RedirectToPage();
            }
            catch (ArgumentException ex) // thrown by the factory's validation
            {
                AnimalError = ex.Message;
                LoadProducts();
                return Page();
            }
        }

        // Called by the form with asp-page-handler="AddItem".
        public IActionResult OnPostAddItem()
        {
            if (Item.Stock is null || !TryParsePrice(Item.Price, out var price))
            {
                ItemError = "Stock and a valid price are required.";
                LoadProducts();
                return Page();
            }

            try
            {
                var item = _itemFactory.CreateNew(
                    Item.Stock.Value,
                    Item.Name ?? "",
                    price,
                    Item.Description ?? ""
                );
                _itemRepo.Add(item);

                TempData["Success"] = $"Item '{item.Name}' saved with id {item.Id}.";
                return RedirectToPage();
            }
            catch (ArgumentException ex)
            {
                ItemError = ex.Message;
                LoadProducts();
                return Page();
            }
        }

        private void LoadProducts()
        {
            Animals = _animalRepo.GetAll().ToList();
            Items = _itemRepo.GetAll().ToList();
        }

        // <input type="number"> always sends a dot ("12.50"). Parsing with the invariant
        // culture avoids a Danish server culture reading it as 1250.
        private static bool TryParsePrice(string? input, out decimal price) =>
            decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out price);
 
    }
}