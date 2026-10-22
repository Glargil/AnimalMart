namespace AnimalMart.Interfaces
{
    public interface IImageStorage
    {
        // Saves the uploaded image and returns its web path, or null when no file was sent.
        string? Save(IFormFile? file);
    }
}
