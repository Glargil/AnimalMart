namespace AnimalMart.Interfaces
{
    public interface IUserService
    {
        User CreateUser(UserCreateDTO dto);
        LoginAttemptResult ValidateLogin(UserLoginDTO dto, string clientIp);
    }
}
