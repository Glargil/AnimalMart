using AnimalMart.Models;

namespace AnimalMart.Interfaces
{
    public interface IUserService
    {
        User CreateUser(UserCreateDTO dto);
    }
}
