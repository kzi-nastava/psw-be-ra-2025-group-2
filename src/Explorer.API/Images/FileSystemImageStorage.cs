using Microsoft.AspNetCore.Http;
namespace Explorer.API.Images;

public class FileSystemImageStorage : IImageStorage
{
    private readonly IWebHostEnvironment _env;
    private const long MaxBytes = 5 * 1024 * 1024; // 5MB
    private static readonly HashSet<string> AllowedExt = new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png" };

    public FileSystemImageStorage(IWebHostEnvironment env) => _env = env;

    public async Task<List<string>> SaveClubImagesAsync(long ownerId, IEnumerable<IFormFile> files, CancellationToken ct)
    {
        var fileList = files?.ToList() ?? new();
        if (!fileList.Any()) throw new ArgumentException("At least one image is required.");

        // folder per create request (guid)
        var batch = Guid.NewGuid().ToString("N");
        var relDir = Path.Combine("uploads", "clubs", ownerId.ToString(), batch);
        var absDir = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), relDir);

        Directory.CreateDirectory(absDir);

        var saved = new List<string>();

        foreach (var file in fileList)
        {
            if (file.Length <= 0) continue;
            if (file.Length > MaxBytes) throw new ArgumentException("Image is too large. Max size is 5MB.");

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext) || !AllowedExt.Contains(ext))
                throw new ArgumentException("Only JPG and PNG images are allowed.");

            // opcionalno: proveri i ContentType
            // if(file.ContentType not in ...)

            var name = $"{Guid.NewGuid():N}{ext}";
            var absPath = Path.Combine(absDir, name);

            await using var stream = File.Create(absPath);
            await file.CopyToAsync(stream, ct);

            // relativna putanja za bazu i front
            saved.Add("/" + Path.Combine(relDir, name).Replace("\\", "/"));
        }

        if (!saved.Any()) throw new ArgumentException("No valid images uploaded.");
        return saved;
    }

    public void DeleteMany(IEnumerable<string> relativePaths)
    {
        if (relativePaths == null) return;

        foreach (var rel in relativePaths)
        {
            if (string.IsNullOrWhiteSpace(rel)) continue;

            var trimmed = rel.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
            var abs = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), trimmed);

            if (File.Exists(abs)) File.Delete(abs);
        }
    }
}
