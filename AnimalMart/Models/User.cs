public class User
{
    public int Id { get; private set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string PasswordHash { get; private set; }
    public ShoppingCart Cart { get; set; } = new ShoppingCart();
}
