using System.Globalization;
using AnimalMart.Interfaces;
using AnimalMart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AnimalMart.Pages.Admin
{
    // Only admins reach this page; everyone else is redirected to /Login.
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class EditAnimalModel : PageModel
    {
        private readonly IAnimalRepo _animalRepo;
        private readonly AnimalFactory _animalFactory;

        public EditAnimalModel(IAnimalRepo animalRepo, AnimalFactory animalFactory)
        {
            _animalRepo = animalRepo;
            _animalFactory = animalFactory;
        }

        // SupportsGet lets ?id=5 bind on GET; on POST the hidden input keeps the value.
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public AnimalCreateDTO Animal { get; set; } = new();

        public string? Error { get; set; }

        public IActionResult OnGet()
        {
            var existing = _animalRepo.GetById(Id);
            if (existing is null)
            {
                // Stale link or unknown id: no information leak, just go back.
                return RedirectToPage("CreateProduct");
            }

            // Prefill the form with the current values.
            Animal = new AnimalCreateDTO
            {
                Name = existing.Name,
                Description = existing.Description,
                Price = existing.Price.ToString("0.00", CultureInfo.InvariantCulture),
                Sex = existing.Sex,
                Species = existing.Species,
                BirthDay = existing.BirthDay,
            };
            return Page();
        }

        public IActionResult OnPost()
        {
            if (Animal.BirthDay is null || !TryParsePrice(Animal.Price, out var price))
            {
                Error = "Birthday and a valid price are required.";
                return Page();
            }

            try
            {
                // CreateNew is reused purely for its validation and trimming,
                // then the id from the route is put back on the object.
                var animal = _animalFactory.CreateNew(
                    Animal.Sex ?? "",
                    Animal.Species ?? "",
                    Animal.BirthDay.Value,
                    price,
                    Animal.Name ?? "",
                    Animal.Description ?? ""
                );
                animal.Id = Id;
                _animalRepo.Update(animal);

                TempData["Success"] = $"Animal '{animal.Name}' updated.";
                return RedirectToPage("CreateProduct");
            }
            catch (ArgumentException ex)
            {
                Error = ex.Message;
                return Page();
            }
            catch (KeyNotFoundException)
            {
                Error = "This animal no longer exists.";
                return Page();
            }
        }

        private static bool TryParsePrice(string? input, out decimal price) =>
            decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out price);
    }
}