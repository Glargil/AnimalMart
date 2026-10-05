public interface IAnimalRepo
{
    public IEnumerable<Animal> GetAll();
    public Animal? GetById(int id);
    public Animal Add(Animal animal);
}
