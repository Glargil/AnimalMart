public interface IAnimalRepo
{
    public IEnumerable<Animal> GetAll();
    public Animal? GetById(int id);
    public Animal Add(Animal animal);
    public void Update(Animal animal);
    public void Delete(int id);
}
