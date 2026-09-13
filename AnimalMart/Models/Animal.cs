public class Animal : Product
{
    public string? Sex { get; set; }
    public string? Species { get; set; }
    public DateOnly BirthDay { get; set; }

    public Animal(
        string sex,
        string species,
        DateOnly birthDay,
        decimal price,
        string name,
        string description
    )
        : base(price, name, description)
    {
        this.Sex = sex;
        this.Species = species;
        this.BirthDay = birthDay;
    }
}
