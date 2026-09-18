namespace AnimalMart.Models
{
    //dto used whenever front end creates a new user object
    public class UserCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Password { get; set; } = string.Empty; // plaintext in; hashed in the controller before it ever reaches the repo
    }
}
