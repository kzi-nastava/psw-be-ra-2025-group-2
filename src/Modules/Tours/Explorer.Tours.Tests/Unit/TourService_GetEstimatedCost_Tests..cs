using AutoMapper;
using Moq;
using Xunit;
using Explorer.Tours.Core.UseCases.Administration;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Mappers;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Tours.API.Dtos;

// ostale zavisnosti TourService-a koje ne koristiš u ovom testu:
using Explorer.Stakeholders.API.Internal;
using Explorer.Tours.API.Public.Execution;
using Explorer.Tours.Core.Domain.Execution;
using Explorer.Tours.Core.Domain.Report;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.API.Internal;
using Explorer.Tours.API.Public;

public class TourService_GetEstimatedCost_Tests
{
    private readonly IMapper _mapper;

    public TourService_GetEstimatedCost_Tests()
    {
        var cfg = new MapperConfiguration(c => c.AddProfile<ToursProfile>());
        _mapper = cfg.CreateMapper();
    }

    [Fact]
    public void GetEstimatedCost_ShouldReturnDto_WhenEstimatedCostExists()
    {
        // Arrange (STUB repo)
        var repo = new Mock<ITourRepository>();
        var tour = new Tour("T1", "Desc", 3, authorId: 11, tags: new[] { "tag" });

        tour.SetEstimatedCost(
            totalPerPerson: 3500,
            currency: "RSD",
            breakdown: new[]
            {
                (EstimatedCostCategory.Transport, 1000m),
                (EstimatedCostCategory.FoodAndDrink, 1500m),
            });

        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(tour);

        // ostale zavisnosti - može stub/mock kao "prazno"
        var service = CreateService(repo.Object);

        // Act
        var dto = service.GetEstimatedCost(1);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(3500m, dto!.TotalPerPerson);
        Assert.Equal("RSD", dto.Currency);
        Assert.Equal(2, dto.Breakdown.Count);
    }

    [Fact]
    public void GetEstimatedCost_ShouldReturnDefault_WhenNoEstimatedCost()
    {
        // Arrange
        var repo = new Mock<ITourRepository>();
        var tour = new Tour("T1", "Desc", 3, authorId: 11, tags: new[] { "tag" });

        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(tour);

        var service = CreateService(repo.Object);

        // Act
        var dto = service.GetEstimatedCost(1);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(0m, dto!.TotalPerPerson);
        Assert.NotEmpty(dto.Disclaimer);
        Assert.Empty(dto.Breakdown);
    }

    [Fact]
    public void GetEstimatedCost_ShouldReturnNull_WhenTourNotFound()
    {
        // Arrange
        var repo = new Mock<ITourRepository>();
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Tour?)null);

        var service = CreateService(repo.Object);

        // Act
        var dto = service.GetEstimatedCost(1);

        // Assert
        Assert.Null(dto);
    }

    private TourService CreateService(ITourRepository tourRepository)
    {
        // sve ostalo što TourService traži, a nije relevantno za ovaj test:
        var userService = Mock.Of<IInternalUserService>();
        var eqRepo = Mock.Of<IEquipmentRepository>();
        var publicKpService = Mock.Of<IPublicKeyPointService>();
        var requestRepo = Mock.Of<IPublicKeyPointRequestRepository>();
        var execRepo = Mock.Of<ITourExecutionRepository>();
        var tokenService = Mock.Of<IInternalTokenService>();

        return new TourService(
            tourRepository,
            _mapper,
            (IEquipmentRepository)eqRepo,
            userService,
            publicKpService,
            requestRepo,
            execRepo,
            tokenService);
    }
}
