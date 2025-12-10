namespace Explorer.Tours.API.Dtos
{
    public class TourEquipmentItemDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsRequiredForTour { get; set; }
    }
}