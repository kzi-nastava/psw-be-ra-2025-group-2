using Microsoft.AspNetCore.Http;

namespace Explorer.API.Dtos;

public class CreateClubRequest
{
    public string Name { get; set; }
    public string Description { get; set; }

    // mora se zvati isto kao key u form-data (images)
    public List<IFormFile> Images { get; set; } = new();
}