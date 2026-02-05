using AutoMapper;
using Explorer.Stakeholders.API.Dtos.Emergency;
using Explorer.Stakeholders.API.Public.Emergency;
using Explorer.Stakeholders.Core.Domain.Emergency;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System.Linq;

namespace Explorer.Stakeholders.Core.UseCases.Emergency
{
    public class EmergencyOverviewService : IEmergencyOverviewService
    {
        private readonly IEmergencyDirectoryRepository _repo;
        private readonly IMapper _mapper;

        public EmergencyOverviewService(IEmergencyDirectoryRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }


        public EmergencyOverviewResponseDto GetOverview(string countryCode)
        {
            var code = new CountryCode(countryCode);
            var directory = _repo.GetByCountry(code);

            // empty response ako nema podataka
            if (directory == null)
            {
                return new EmergencyOverviewResponseDto
                {
                    CountryCode = code.Value,
                    Hospitals = new(),
                    PoliceStations = new(),
                    FireStations = new(),
                    Embassies = new(),
                    PhraseCategories = new List<PhraseCategoryDto>
            
                        {
                            new PhraseCategoryDto { Category = "Medicine", Phrases = new() },
                            new PhraseCategoryDto { Category = "Police", Phrases = new() }
                        },

                    Instructions = "No available instructions for the selected country.",
                    Disclaimer = "Shown data is only for informative purpose and non-binding."
                };
            }

            var hospitals = directory.GetHospitals().Select(_mapper.Map<EmergencyPlaceDto>).ToList();
            var police = directory.GetPoliceStations().Select(_mapper.Map<EmergencyPlaceDto>).ToList();
            var fireStations = directory.GetFireStations().Select(_mapper.Map<EmergencyPlaceDto>).ToList();
            var embassies = directory.Embassies.Select(_mapper.Map<EmbassyDto>).ToList();

            var medicine = directory.GetPhrases(EmergencyPhraseCategory.Medicine)
                .Select(_mapper.Map<PhraseDto>).ToList();

            var policePhrases = directory.GetPhrases(EmergencyPhraseCategory.Police)
                .Select(_mapper.Map<PhraseDto>).ToList();

            var phraseCategories = new List<PhraseCategoryDto>
            {
                new PhraseCategoryDto
                {
                    Category = "Medicine",
                    Phrases = directory.GetPhrases(EmergencyPhraseCategory.Medicine)
                        .Select(_mapper.Map<PhraseDto>)
                        .ToList()
                },
                new PhraseCategoryDto
                {
                    Category = "Police",
                    Phrases = directory.GetPhrases(EmergencyPhraseCategory.Police)
                        .Select(_mapper.Map<PhraseDto>)
                        .ToList()
                }
            };


            return new EmergencyOverviewResponseDto
            {
                CountryCode = directory.Country.Value,
                Hospitals = hospitals,
                PoliceStations = police,
                FireStations = fireStations,
                Embassies = embassies,
                PhraseCategories = phraseCategories,
                Instructions = directory.Instructions,
                Disclaimer = directory.Disclaimer
            };
        }

    }
}
