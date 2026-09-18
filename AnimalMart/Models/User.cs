public class User
{
    //id public setter for repo methods -egil
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; } = string.Empty;
    public string PasswordHash { get; set; } 
    public ShoppingCart Cart { get; set; } = new ShoppingCart();
}
