using System.Data;
public abstract class ProductFactory
{
    // Rebuilds an existing product from a database row.
    public abstract Product Create(IDataRecord row);

    //Shared checks for the base fields, used by the subclasses' CreateNew.
    protected static void ValidateBase(decimal price, string name, string description)
    {
        if (price < 0) throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required.", nameof(description));
    }
}

