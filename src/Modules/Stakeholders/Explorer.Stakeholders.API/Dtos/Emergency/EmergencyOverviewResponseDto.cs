using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos.Emergency
{

    public class EmergencyOverviewResponseDto
    {
        public string CountryCode { get; set; } = "";
        public List<EmergencyPlaceDto> Hospitals { get; set; } = new();
        public List<EmergencyPlaceDto> PoliceStations { get; set; } = new();

        public List<EmergencyPlaceDto> FireStations { get; set; } = new();


        public List<EmbassyDto> Embassies { get; set; } = new();
        public List<PhraseCategoryDto> PhraseCategories { get; set; } = new();

        public string Instructions { get; set; } = "";
        public string Disclaimer { get; set; } = "";
    }
}
