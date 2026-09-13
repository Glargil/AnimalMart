public class Item : Product
{
    public int Stock { get; set; }

    public Item(int stock, string name, decimal price, string description)
        : base(price, name, description)
    {
        this.Stock = stock;
    }
}
