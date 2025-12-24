using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public enum DiaryStatus
{
    Draft,
    Published,
    Archived
}

public class Diary : Entity
{
    public long UserId { get; private set; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DiaryStatus Status { get; private set; }
    public string Country { get; private set; }
    public string? City { get; private set; }

    public Diary(long userId, string name, string? country = null, string? city = null)
    {
        name = name?.Trim();
        country = country?.Trim();
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required.");
       // if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException("Country required.");
        UserId = userId;
        Name = name;
        Country = country;
        City = city;
        CreatedAt = DateTime.UtcNow;
        Status = DiaryStatus.Draft;
    }
}
