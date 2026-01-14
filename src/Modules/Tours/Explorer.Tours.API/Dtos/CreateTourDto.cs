using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class CreateTourDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int Difficulty { get; set; }
        public long AuthorId { get; set; }
        public List<string>? Tags { get; set; }
        public List<KeyPointDto> KeyPoints { get; set; } = new();
        public List<TourDurationDto> Durations { get; set; } = new();
        public List<long> RequiredEquipmentIds { get; set; } = new();

        public decimal? EstimatedCostTotalPerPerson { get; set; }
        public string? EstimatedCostCurrency { get; set; }
        public List<EstimatedCostItemDto>? EstimatedCostBreakdown { get; set; }
    }
}
