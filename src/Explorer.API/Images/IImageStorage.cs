using Microsoft.AspNetCore.Http;
namespace Explorer.API.Images;

public interface IImageStorage
{
    Task<List<string>> SaveClubImagesAsync(long ownerId, IEnumerable<IFormFile> files, CancellationToken ct);
    void DeleteMany(IEnumerable<string> relativePaths);
}
  