public interface IUserRepository
{
    public IEnumerable<User> GetAll();
    public User? GetById(int id);
    public User Add(User user);
};
