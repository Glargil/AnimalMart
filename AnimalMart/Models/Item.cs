public class Item : Product
{
    public int Stock { get; set; }

    public Item(int id, int stock, string name, decimal price, string description,
        string? imgPath = null)
        : base(id, price, name, description, imgPath)
    {
        Stock = stock;
    }
}
