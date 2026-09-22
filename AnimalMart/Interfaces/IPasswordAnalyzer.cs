public interface IPasswordAnalyzer
{  
  PasswordResult Analyze(string password, string? username);
}