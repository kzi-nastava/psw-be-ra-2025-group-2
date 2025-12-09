namespace Explorer.Tours.API.Dtos
{
    public class UpdateTourEquipmentDto
    {
        public List<long> EquipmentIds { get; set; } = new();
    }
}