using AutoMapper;
using Explorer.Stakeholders.Core.Domain.Emergency;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Mappers;              
using Explorer.Stakeholders.Core.UseCases.Emergency;
using Moq;
using Shouldly;
using Xunit;

namespace Explorer.Stakeholders.Tests.Unit
{
    public class EmergencyOverviewServiceUnitTests
    {
        [Fact]
        public void Returns_empty_when_directory_not_found()
        {
            var repo = new Mock<IEmergencyDirectoryRepository>();
            repo.Setup(r => r.GetByCountry(It.IsAny<CountryCode>()))
                .Returns((EmergencyDirectory?)null);

            
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<StakeholderProfile>();
            });
            var mapper = mapperConfig.CreateMapper();

            var service = new EmergencyOverviewService(repo.Object, mapper);

            var dto = service.GetOverview("DE");

            dto.CountryCode.ShouldBe("DE");
            dto.Hospitals.Count.ShouldBe(0);
            dto.PoliceStations.Count.ShouldBe(0);
            dto.FireStations.Count.ShouldBe(0);
            dto.Disclaimer.ShouldNotBeNullOrWhiteSpace();
            dto.Instructions.ShouldNotBeNullOrWhiteSpace();

            dto.Embassies.ShouldNotBeNull();
            dto.Embassies.Count.ShouldBe(0);

            dto.PhraseCategories.ShouldNotBeNull();
            dto.PhraseCategories.Count.ShouldBe(2);

            dto.PhraseCategories.Any(c => c.Category == "Medicine").ShouldBeTrue();
            dto.PhraseCategories.First(c => c.Category == "Medicine").Phrases.Count.ShouldBe(0);

            dto.PhraseCategories.Any(c => c.Category == "Police").ShouldBeTrue();
            dto.PhraseCategories.First(c => c.Category == "Police").Phrases.Count.ShouldBe(0);

        }
    }
}
