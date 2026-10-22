public class Item : Product
{
    public int Stock { get; set; }

    public Item(int id, int stock, string name, decimal price, string description)
        : base(id, price, name, description)
    {
        Stock = stock;
    }
}
