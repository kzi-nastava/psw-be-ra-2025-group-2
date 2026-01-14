using AutoMapper;
using Moq;
using Xunit;
using Explorer.Tours.Core.UseCases.Administration;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Mappers;
using Explorer.Tours.API.Dtos;

using Explorer.Stakeholders.API.Internal;
using Explorer.Tours.API.Public.Execution;
using Explorer.Payments.API.Internal;
using Explorer.Tours.API.Public;

public class TourService_Update_ShouldCallUpdateAsync
{
    [Fact]
    public void Update_ShouldCallRepositoryUpdate()
    {
        // Arrange
        var mapper = new MapperConfiguration(c => c.AddProfile<ToursProfile>()).CreateMapper();
        var repo = new Mock<ITourRepository>();

        var tour = new Tour("T1", "Desc", 3, 11, new[] { "tag" });

        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(tour);
        repo.Setup(r => r.UpdateAsync(It.IsAny<Tour>())).Returns(Task.CompletedTask);

        var service = CreateService(repo.Object, mapper);

        var dto = new UpdateTourDto
        {
            Name = "T1",
            Description = "Desc",
            Difficulty = 3,
            Tags = new List<string> { "tag" },
            Price = 0,
            KeyPoints = new(),
            EstimatedCostTotalPerPerson = 3500,
            EstimatedCostCurrency = "RSD",
            EstimatedCostBreakdown = new List<EstimatedCostItemDto>()
        };

        // Act
        service.Update(1, dto);

        // Assert (MOCK verify)
        repo.Verify(r => r.UpdateAsync(It.Is<Tour>(t => t.Id == tour.Id)), Times.Once);
    }

    private TourService CreateService(ITourRepository tourRepository, IMapper mapper)
    {
        return new TourService(
            tourRepository,
            mapper,
            Mock.Of<IEquipmentRepository>(),
            Mock.Of<IInternalUserService>(),
            Mock.Of<IPublicKeyPointService>(),
            Mock.Of<IPublicKeyPointRequestRepository>(),
            Mock.Of<ITourExecutionRepository>(),
            Mock.Of<IInternalTokenService>());
    }
}
