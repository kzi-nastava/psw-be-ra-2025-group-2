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
            dto.Disclaimer.ShouldNotBeNullOrWhiteSpace();
            dto.Instructions.ShouldNotBeNullOrWhiteSpace();
        }
    }
}
