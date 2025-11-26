namespace Explorer.Tours.API.Dtos
{
    public class TouristMapPointsDto
    {
        public IEnumerable<MonumentDto> Monuments { get; set; }
        public IEnumerable<TouristObjectDto> TouristObjects { get; set; }
    }
}