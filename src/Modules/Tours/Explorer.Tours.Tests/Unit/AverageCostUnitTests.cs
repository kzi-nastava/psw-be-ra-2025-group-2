using AutoMapper;
using Explorer.Payments.API.Internal;
using Explorer.Payments.API.Public;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Public.Execution;
using Explorer.Tours.Core.Domain;
using Explorer.API.Controllers.Tourist;

using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Mappers;
using Explorer.Tours.Core.UseCases.Administration;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Unit;

public class AverageCostUnitTests
{
    private static IMapper CreateMapper()
    {
        var cfg = new MapperConfiguration(c => c.AddProfile<ToursProfile>());
        return cfg.CreateMapper();
    }

    [Fact]
    public void Create_calls_estimator_once_and_sets_average_cost()
    {
        // Arrange
        var repo = new Mock<ITourRepository>();

        Tour? saved = null;
        repo.Setup(r => r.AddAsync(It.IsAny<Tour>()))
            .Callback<Tour>(t => saved = t)
            .ReturnsAsync((Tour t) => t);

        var estimator = new Mock<IAverageCostEstimatorService>();
        var expected = AverageCost.Create(
            "RSD",
            AverageCostBreakdown.Create(100, 200, 300, 400),
            "stub disclaimer"
        );
        estimator.Setup(e => e.Estimate(It.IsAny<Tour>())).Returns(expected);

        var service = new TourService(
            repo.Object,
            CreateMapper(),
            new Mock<IEquipmentRepository>().Object,
            new Mock<IInternalUserService>().Object,
            new Mock<IPublicKeyPointService>().Object,
            new Mock<IPublicKeyPointRequestRepository>().Object,
            new Mock<ITourExecutionRepository>().Object,
            new Mock<IInternalTokenService>().Object,
            estimator.Object
        );

        var dto = new CreateTourDto
        {
            Name = "t",
            Description = "d",
            Difficulty = 3,
            AuthorId = 1,
            Price = 0,
            Tags = new() { "tag" },
            KeyPoints = new()
        {
            new KeyPointDto
            {
                OrdinalNo = 1,
                Name = "Museum",
                Description = "x",
                ImageUrl = "http://img",
                Latitude = 45,
                Longitude = 19,
                AuthorId = 1,
                OsmClass = "tourism",
                OsmType = "museum"
            },
            new KeyPointDto
            {
                OrdinalNo = 2,
                Name = "Cafe",
                Description = "y",
                ImageUrl = "http://img2",
                Latitude = 45.1,
                Longitude = 19.1,
                AuthorId = 1,
                OsmClass = "amenity",
                OsmType = "cafe"
            }
        }
        };

        // Act
        var created = service.Create(dto);

        // Assert
        estimator.Verify(e => e.Estimate(It.IsAny<Tour>()), Times.Once);

        saved.ShouldNotBeNull();
        saved!.AverageCost.ShouldNotBeNull();
        saved.AverageCost!.Disclaimer.ShouldBe("stub disclaimer");
        saved.AverageCost.Breakdown.ShouldNotBeNull();
    }

    [Fact]
    public void Update_calls_estimator_once()
    {
        // Arrange
        var existing = new Tour("t", "d", 3, 1, new[] { "tag" });

        existing.AddKeyPoint(new KeyPoint(1, "kp1", "desc", "", "http://img", 45, 19, 1,
            osmClass: "tourism", osmType: "museum"));

        existing.AddKeyPoint(new KeyPoint(2, "kp2", "desc2", "", "http://img2", 45.1, 19.1, 1,
            osmClass: "amenity", osmType: "cafe"));

        var repo = new Mock<ITourRepository>();
        repo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(existing);

        Tour? updated = null;
        repo.Setup(r => r.UpdateAsync(It.IsAny<Tour>()))
            .Callback<Tour>(t => updated = t)
            .Returns(Task.CompletedTask);

        var estimator = new Mock<IAverageCostEstimatorService>();
        estimator.Setup(e => e.Estimate(It.IsAny<Tour>()))
                 .Returns(AverageCost.Create("RSD", AverageCostBreakdown.Create(1, 2, 3, 4), "d"));

        var service = new TourService(
            repo.Object,
            CreateMapper(),
            new Mock<IEquipmentRepository>().Object,
            new Mock<IInternalUserService>().Object,
            new Mock<IPublicKeyPointService>().Object,
            new Mock<IPublicKeyPointRequestRepository>().Object,
            new Mock<ITourExecutionRepository>().Object,
            new Mock<IInternalTokenService>().Object,
            estimator.Object
        );

        var update = new UpdateTourDto
        {
            Name = "t2",
            Description = "d2",
            Difficulty = 4,
            Price = 10,
            Tags = new() { "tag" },
            LengthKm = 10m,
            KeyPoints = new(),
            Durations = new()
        };

        // Act
        service.Update(10, update);

        // Assert
        estimator.Verify(e => e.Estimate(It.IsAny<Tour>()), Times.Once);

        updated.ShouldNotBeNull();
        updated!.AverageCost.ShouldNotBeNull();
    }


    [Fact]
    public void GetTourInfo_returns_average_cost_when_service_returns_it()
    {
        // Arrange
        var tourService = new Mock<ITourService>();
        var payment = new Mock<IPaymentRecordService>();
        var exec = new Mock<ITourExecutionService>();
        var tokenRepo = new Mock<ITourPurchaseTokenRepository>();

        var tourId = 123;

        tourService.Setup(s => s.GetFullTourInfo(tourId)).Returns(new FullTourInfoDto
        {
            Id = tourId,
            Name = "Tura",
            Difficulty = 2,
            AverageCost = new AverageCostDto
            {
                Currency = "RSD",
                Disclaimer = "Informativno",
                Breakdown = new AverageCostBreakdownDto
                {
                    Tickets = 500,
                    Transport = 350,
                    FoodAndDrink = 600,
                    Other = 200
                },
                TotalPerPerson = 1650
            }
        });

        var controller = new TourController(
            tourService.Object,
            payment.Object,
            exec.Object,
            tokenRepo.Object
        );

        // Act
        var result = controller.GetTourInfo(tourId);

        // Assert
        var ok = result.Result as OkObjectResult;
        ok.ShouldNotBeNull();

        var dto = ok.Value as FullTourInfoDto;
        dto.ShouldNotBeNull();

        dto.AverageCost.ShouldNotBeNull();
        dto.AverageCost!.TotalPerPerson.ShouldBe(1650);
        dto.AverageCost.Disclaimer.ShouldNotBeNullOrWhiteSpace();
        dto.AverageCost.Breakdown.ShouldNotBeNull();
    }
}
