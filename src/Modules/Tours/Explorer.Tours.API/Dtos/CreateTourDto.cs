using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class CreateTourDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Difficulty { get; set; }
        public long AuthorId { get; set; }
        public List<string>? Tags { get; set; }
        public List<KeyPointDto> KeyPoints { get; set; } = new();
        public List<long> RequiredEquipmentIds { get; set; } = new();

    }
}
