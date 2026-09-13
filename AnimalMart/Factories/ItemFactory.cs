public class ItemFactory : ProductFactory
{
    public Item CreateProduct(int stock, string name, decimal price, string description)
    {
        return new Item(stock, name, price, description);
    }
}
