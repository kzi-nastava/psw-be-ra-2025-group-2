using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Dtos.Encounter
{
    public class CreateEncounterDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [JsonPropertyName("XP")]
        public int XP { get; set; }
        public string Type { get; set; }
    }
}
