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
                    Embassies = new(),
                    PhraseCategories = new List<PhraseCategoryDto>
            
                        {
                            new PhraseCategoryDto { Category = "Medicina", Phrases = new() },
                            new PhraseCategoryDto { Category = "Policija", Phrases = new() }
                        },

                    Instructions = "Nema dostupnih uputstava za izabranu državu.",
                    Disclaimer = "Prikazani podaci su informativnog karaktera i neobavezujući."
                };
            }

            var hospitals = directory.GetHospitals().Select(_mapper.Map<EmergencyPlaceDto>).ToList();
            var police = directory.GetPoliceStations().Select(_mapper.Map<EmergencyPlaceDto>).ToList();
            var embassies = directory.Embassies.Select(_mapper.Map<EmbassyDto>).ToList();

            var medicine = directory.GetPhrases(EmergencyPhraseCategory.Medicine)
                .Select(_mapper.Map<PhraseDto>).ToList();

            var policePhrases = directory.GetPhrases(EmergencyPhraseCategory.Police)
                .Select(_mapper.Map<PhraseDto>).ToList();

            var phraseCategories = new List<PhraseCategoryDto>
            {
                new PhraseCategoryDto
                {
                    Category = "Medicina",
                    Phrases = directory.GetPhrases(EmergencyPhraseCategory.Medicine)
                        .Select(_mapper.Map<PhraseDto>)
                        .ToList()
                },
                new PhraseCategoryDto
                {
                    Category = "Policija",
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
                Embassies = embassies,
                PhraseCategories = phraseCategories,
                Instructions = directory.Instructions,
                Disclaimer = directory.Disclaimer
            };
        }

    }
}
