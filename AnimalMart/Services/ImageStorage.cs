using AnimalMart.Interfaces;

namespace AnimalMart.Services
{
    public class ImageStorage : IImageStorage
    {
        private const long MaxBytes = 5 * 1024 * 1024; // 5 MB
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        private readonly string _uploadFolder;

        public ImageStorage(IWebHostEnvironment env)
        {
            // wwwroot/uploads, kept apart from the seeded images in wwwroot/img
            _uploadFolder = Path.Combine(env.WebRootPath, "uploads");
            Directory.CreateDirectory(_uploadFolder); // does nothing if it already exists
        }

        public string? Save(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null; // image is optional

            if (file.Length > MaxBytes)
                throw new ArgumentException("Image must be 5 MB or smaller.");

            // Check the extension, not file.ContentType: the browser sets ContentType and it can be faked.
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new ArgumentException("Image must be a .jpg, .jpeg, .png or .webp file.");

            // Never use the uploaded file name: a random name can't overwrite files or contain "../"
            var fileName = Guid.NewGuid().ToString("N") + extension;

            using var stream = File.Create(Path.Combine(_uploadFolder, fileName));
            file.CopyTo(stream);

            return "/uploads/" + fileName;
        }
    }
}
