using System.Globalization;
using AnimalMart.Interfaces;
using AnimalMart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AnimalMart.Pages.Admin
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class EditItemModel : PageModel
    {
        private readonly IItemRepo _itemRepo;
        private readonly ItemFactory _itemFactory;

        public EditItemModel(IItemRepo itemRepo, ItemFactory itemFactory)
        {
            _itemRepo = itemRepo;
            _itemFactory = itemFactory;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public ItemCreateDTO Item { get; set; } = new();

        public string? Error { get; set; }

        public IActionResult OnGet()
        {
            var existing = _itemRepo.GetById(Id);
            if (existing is null)
            {
                return RedirectToPage("CreateProduct");
            }

            Item = new ItemCreateDTO
            {
                Name = existing.Name,
                Description = existing.Description,
                Price = existing.Price.ToString("0.00", CultureInfo.InvariantCulture),
                Stock = existing.Stock,
            };
            return Page();
        }

        public IActionResult OnPost()
        {
            if (Item.Stock is null || !TryParsePrice(Item.Price, out var price))
            {
                Error = "Stock and a valid price are required.";
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
                item.Id = Id;
                _itemRepo.Update(item);

                TempData["Success"] = $"Item '{item.Name}' updated.";
                return RedirectToPage("CreateProduct");
            }
            catch (ArgumentException ex)
            {
                Error = ex.Message;
                return Page();
            }
            catch (KeyNotFoundException)
            {
                Error = "This item no longer exists.";
                return Page();
            }
        }

        private static bool TryParsePrice(string? input, out decimal price) =>
            decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out price);
    }
}