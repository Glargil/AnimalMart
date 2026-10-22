public interface IItemRepo
{
    public IEnumerable<Item> GetAll();
    public IEnumerable<Item> Search(string term);
    public Item? GetById(int id);
    public Item Add(Item item);
};
