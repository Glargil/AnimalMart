public enum LoginResult
{
    Success,
    InvalidCredentials,
    LockedOut,
}

public class LoginAttemptResult
{
    public LoginResult Result { get; set; }
    public User? User { get; set; }
}
