public interface IItemRepo
{
    public IEnumerable<Item> GetAll();
    public Item? GetById(int id);
    public Item Add(Item item);
};
