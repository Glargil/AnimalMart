using AnimalMart.Interfaces;
using AnimalMart.Security;

namespace AnimalMart.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _repo;

        public UserService(IUserRepo repo)
        {
            _repo = repo;
        }

        public User CreateUser(UserCreateDTO dto)
        {
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = Argon2PasswordHasher.HashPassword(dto.Password)
            };

            return _repo.CreateUser(user);
        }
    }
}
