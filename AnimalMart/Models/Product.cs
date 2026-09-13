public abstract class Product
{
    public int Id { get; private set; }
    public decimal Price { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public Product(decimal price, string name, string description)
    {
        this.Description = description;
        this.Name = name;
        this.Price = price;
    }
}
