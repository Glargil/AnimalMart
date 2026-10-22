using System.Data;

public class ItemFactory : ProductFactory
{
    // Rebuilds an existing item from a database row.
    // this method is used when loading items from the database.
    public override Item Create(IDataRecord row)
    {
        return new Item(
            row.GetInt32(row.GetOrdinal("id")),
            row.GetInt32(row.GetOrdinal("stock")),
            row.GetString(row.GetOrdinal("name")),
            row.GetDecimal(row.GetOrdinal("price")),
            row.GetString(row.GetOrdinal("description"))
        );
    }

    // Builds a new, unsaved animal from admin form input (id = 0 until saved).
    //this method is used when creating a new animal from a form on frontend
    public Item CreateNew(int stock, string name, decimal price, string description)
    {
        ValidateBase(price, name, description);
        if (stock < 0) throw new ArgumentOutOfRangeException(nameof(stock), "Stock cannot be negative.");

        return new Item(0, stock, name.Trim(), price, description.Trim());
    }
}