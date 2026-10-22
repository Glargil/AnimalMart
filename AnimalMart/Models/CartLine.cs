public class CartLine
{
	public Product Product { get; set; } = null!;
	public ProductType ProductType { get; set; }
	public int Quantity { get; set; }

	public decimal Subtotal => Product.Price * Quantity;
}
