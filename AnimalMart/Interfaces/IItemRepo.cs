public interface IItemRepo
{
    public IEnumerable<Item> GetAll();
    public Item? GetById(int id);
    public Item Add(Item item);
    public void Update(Item item);
    public void Delete(int id);
};
