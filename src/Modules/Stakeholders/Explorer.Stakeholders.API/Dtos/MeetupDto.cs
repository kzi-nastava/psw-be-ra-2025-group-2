namespace Explorer.Stakeholders.API.Dtos;

public class MeetupDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; } // Markdown
    public DateTime Date { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public long CreatorId { get; set; }
}