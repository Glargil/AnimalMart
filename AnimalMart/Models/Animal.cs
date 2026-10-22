public class Animal : Product
{
    public string? Sex { get; set; }
    public string? Species { get; set; }
    public DateOnly BirthDay { get; set; }

    public Animal(
        int id,
        string sex,
        string species,
        DateOnly birthDay,
        decimal price,
        string name,
        string description,
        string? imgPath = null
    )
        : base(id, price, name, description, imgPath)
    {
        Sex = sex;
        Species = species;
        BirthDay = birthDay;
    }
}
