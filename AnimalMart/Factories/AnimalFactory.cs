public class AnimalFactory : ProductFactory
{
    public Animal CreateProduct(
        string sex,
        string species,
        DateOnly birthDay,
        decimal price,
        string name,
        string description
    )
    {
        return new Animal(sex, species, birthDay, price, name, description);
    }
}
