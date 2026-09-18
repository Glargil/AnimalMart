using AnimalMart.Interfaces;
using Isopoh.Cryptography.Argon2;

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
                PasswordHash = Argon2.Hash(dto.Password)
            };

            return _repo.CreateUser(user);
        }
    }
}
