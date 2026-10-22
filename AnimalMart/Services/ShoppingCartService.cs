using AnimalMart.Extensions;
using AnimalMart.Interfaces;

namespace AnimalMart.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private const string CartSessionKey = "Cart";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAnimalRepo _animalRepo;
        private readonly IItemRepo _itemRepo;

        public ShoppingCartService(
            IHttpContextAccessor httpContextAccessor,
            IAnimalRepo animalRepo,
            IItemRepo itemRepo
        )
        {
            _httpContextAccessor = httpContextAccessor;
            _animalRepo = animalRepo;
            _itemRepo = itemRepo;
        }

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        private List<CartItem> GetItems()
        {
            return Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
        }

        private void SaveItems(List<CartItem> items)
        {
            Session.Set(CartSessionKey, items);
        }

        public ShoppingCart GetCart()
        {
            var items = GetItems();
            var cart = new ShoppingCart();

            foreach (var item in items)
            {
                Product? product =
                    item.ProductType == ProductType.Animal
                        ? _animalRepo.GetById(item.ProductId)
                        : _itemRepo.GetById(item.ProductId);

                if (product == null)
                {
                    continue;
                }

                cart.Lines.Add(
                    new CartLine
                    {
                        Product = product,
                        ProductType = item.ProductType,
                        Quantity = item.Quantity,
                    }
                );
            }
            return cart;
        }

        public int GetItemCount()
        {
            return GetItems().Sum(i => i.Quantity);
        }

        public void AddProduct(int productId, ProductType productType, int qunatity = 1)
        {
            var items = GetItems();
            var existing = items.FirstOrDefault(i =>
                i.ProductId == productId && i.ProductType == productType
            );

            if (existing != null)
            {
                existing.Quantity += qunatity;
            }
            else
            {
                items.Add(
                    new CartItem
                    {
                        ProductId = productId,
                        ProductType = productType,
                        Quantity = qunatity,
                    }
                );
            }
            SaveItems(items);
        }

        public void RemoveProduct(int productId, ProductType productType)
        {
            var items = GetItems();
            items.RemoveAll(i => i.ProductId == productId && i.ProductType == productType);
            SaveItems(items);
        }

        public void UpdateQuantity(int productId, ProductType productType, int quantity)
        {
            if (quantity <= 0)
            {
                RemoveProduct(productId, productType);
                return;
            }

            var items = GetItems();
            var existing = items.FirstOrDefault(i =>
                i.ProductId == productId && i.ProductType == productType
            );

            if (existing != null)
            {
                existing.Quantity = quantity;
                SaveItems(items);
            }
        }

        public void ClearCart()
        {
            SaveItems(new List<CartItem>());
        }
    }
}
