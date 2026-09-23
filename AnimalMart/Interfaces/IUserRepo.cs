namespace AnimalMart.Interfaces
{
    public interface IUserRepo
    {
        User CreateUser(User user);
        User GetUser(int userId);
        User UpdateUser(User user);
        void DeleteUser(int userId);
        List<User> GetAllUsers();
    }
}
