public class ShoppingCart
{
    public List<Product> Products { get; set; }
    public decimal TotalPrice => Products.Sum(p => p.Price);

    public ShoppingCart()
    {
        Products = new List<Product>();
    }

    public void AddProduct(Product product)
    {
        Products.Add(product);
    }
}
