namespace Explorer.Stakeholders.API.Dtos;

public class DiaryDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "Draft";
    public string Country { get; set; } = string.Empty;
    public string? City { get; set; }
}