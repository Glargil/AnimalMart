public interface IShoppingCartService
{
	public ShoppingCart GetCart();
	public int GetItemCount();
	public void AddProduct(int productId, ProductType productType, int quantity = 1);
	public void RemoveProduct(int productId, ProductType productType);
	public void UpdateQuantity(int productId, ProductType productType, int quantity);
	public void ClearCart();
} 
