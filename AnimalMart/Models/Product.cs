public abstract class Product
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public Product(int id, decimal price, string name, string description)
    {
        Id = id;
        Description = description;
        Name = name;
        Price = price;
    }
}
