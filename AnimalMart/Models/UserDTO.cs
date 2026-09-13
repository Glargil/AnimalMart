public class UserDTO
{
    public int Id { get; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public UserDTO(User user)
    {
        Id = user.Id;
        Name = user.Name;
        Email = user.Email;
        PhoneNumber = user.PhoneNumber;
    }
}
