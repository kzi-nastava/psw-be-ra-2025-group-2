namespace Explorer.Tours.API.Dtos;
public class MonumentDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int YearOfCreation { get; set; }
    public string State { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
}
