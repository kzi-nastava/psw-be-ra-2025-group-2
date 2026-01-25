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
                    Instructions = "Nema dostupnih uputstava za izabranu državu.",
                    Disclaimer = "Prikazani podaci su informativnog karaktera i neobavezujući."
                };
            }

            
            var hospitals = directory.GetHospitals().Select(_mapper.Map<EmergencyPlaceDto>).ToList();
            var police = directory.GetPoliceStations().Select(_mapper.Map<EmergencyPlaceDto>).ToList();

            return new EmergencyOverviewResponseDto
            {
                CountryCode = directory.Country.Value,
                Hospitals = hospitals,
                PoliceStations = police,
                Instructions = directory.Instructions,
                Disclaimer = directory.Disclaimer
            };
        }
    }
}
