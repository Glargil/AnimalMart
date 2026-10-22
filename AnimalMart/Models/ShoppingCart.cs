public class ShoppingCart
{
    public List<CartLine> Lines { get; set; }
    public decimal TotalPrice => Lines.Sum(l => l.Subtotal);
    public int TotalQuantity => Lines.Sum(l => l.Quantity);

    public ShoppingCart()
    {
        Lines = new List<CartLine>();
    }
}
