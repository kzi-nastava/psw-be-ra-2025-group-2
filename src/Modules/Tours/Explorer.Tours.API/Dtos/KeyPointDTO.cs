namespace Explorer.Tours.API.Dtos;
public class KeyPointDto
{
    public long Id { get; set; }
    public int OrdinalNo { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? SecretText { get; set; }
    public string ImageUrl { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public long AuthorId { get; set; }
    public string PublicStatus { get; set; } = "Private";
    public bool IsPublic { get; set; }
    public class CreateKeyPointDto
    {
        public int OrdinalNo { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SecretText { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool SuggestForPublicUse { get; set; } = false;
    }
}
