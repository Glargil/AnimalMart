using AnimalMart.Interfaces;
using Isopoh.Cryptography.Argon2;

namespace AnimalMart.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _repo;
        private readonly ILoginAttemptTracker _loginAttemptTracker;

        
        public UserService(IUserRepo repo, ILoginAttemptTracker loginAttemptTracker)
        {
            _repo = repo;
            _loginAttemptTracker = loginAttemptTracker;
        }

        public User CreateUser(UserCreateDTO dto)
        {
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = Argon2.Hash(dto.Password),
            };

            return _repo.CreateUser(user);
        }


        public LoginAttemptResult ValidateLogin(UserLoginDTO dto, string clientIp)
        {
            if (_loginAttemptTracker.IsLockedOut(clientIp))
            {
                return new LoginAttemptResult { Result = LoginResult.LockedOut, User = null };
            }
            var user = _repo.GetUserByEmail(dto.Email);
            var passwordOk = user != null && Argon2.Verify(user.PasswordHash, dto.Password);

            if (!passwordOk)
            {
                _loginAttemptTracker.RecordFailure(clientIp);
                return new LoginAttemptResult
                {
                    Result = LoginResult.InvalidCredentials,
                    User = null,
                };
            }

            _loginAttemptTracker.RecordSuccess(clientIp);
            return new LoginAttemptResult { Result = LoginResult.Success, User = user };
        }
    }
}
