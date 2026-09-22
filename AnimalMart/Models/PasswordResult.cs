public enum PasswordCategory
{
    Weak,
    Medium,
    Strong
}

public record PasswordResult(
    int Score,
    PasswordCategory Category,
    List<string> Feedback,
    bool MeetsPolicy);