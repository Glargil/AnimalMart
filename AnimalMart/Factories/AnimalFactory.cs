using System.Data;

public class AnimalFactory : ProductFactory
{
    // Rebuilds an existing animal from a database row.
    // this method is used when loading animals from the database.
    public override Animal Create(IDataRecord row)
    {
        return new Animal(
            row.GetInt32(row.GetOrdinal("id")),
            row.GetString(row.GetOrdinal("sex")),
            row.GetString(row.GetOrdinal("species")),
            DateOnly.FromDateTime(row.GetDateTime(row.GetOrdinal("birthday"))),
            row.GetDecimal(row.GetOrdinal("price")),
            row.GetString(row.GetOrdinal("name")),
            row.GetString(row.GetOrdinal("description"))
        );
    }

    // Builds a new, unsaved animal from admin form input (id = 0 until saved).
    //this method is used when creating a new animal from a form on frontend
    public Animal CreateNew(string sex, string species, DateOnly birthDay,
                            decimal price, string name, string description)
    {
        ValidateBase(price, name, description);
        if (string.IsNullOrWhiteSpace(sex)) throw new ArgumentException("Sex is required.", nameof(sex));
        if (string.IsNullOrWhiteSpace(species)) throw new ArgumentException("Species is required.", nameof(species));
        if (birthDay > DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentOutOfRangeException(nameof(birthDay), "Birthday cannot be in the future.");

        return new Animal(0, sex.Trim(), species.Trim(), birthDay, price, name.Trim(), description.Trim());
    }
}