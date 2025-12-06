using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class TourExecutionDto
    {
        public long Id { get; set; }
        public long TourId { get; set; }
        public long TouristId { get; set; }
        public List<KeyPointDto> KeyPointData { get; set; }
    }
}
