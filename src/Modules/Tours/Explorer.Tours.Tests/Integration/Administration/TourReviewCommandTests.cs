using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Public.Execution;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourReviewCommandTests : BaseToursIntegrationTest
{
    public TourReviewCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-23"); // Turista -23
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var newEntity = new TourReviewDto
        {
            TourId = -1,
            ExecutionId = -2, // Ovo mora da postoji u d-tour-executions.sql
            Rating = 5,
            Comment = "Odlična tura, preporučujem!",
            Images = new List<string> { "new_image.jpg" },
            TouristId = -23,
            ReviewDate = DateTime.UtcNow,
            CompletedPercentage = 66
        };

        // Act
        var response = controller.Create(newEntity).Result;

        if (response is ObjectResult errorResult && errorResult.StatusCode != 200)
        {
            throw new Exception($"CREATE NIJE USPEO! Status: {errorResult.StatusCode}, Poruka: {errorResult.Value}");
        }

        var result = (response as OkObjectResult)?.Value as TourReviewDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Comment.ShouldBe(newEntity.Comment);
    }

    [Fact]
    public void Create_fails_invalid_rating()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");
        var newEntity = new TourReviewDto
        {
            TourId = -3,
            ExecutionId = -1,
            Rating = 10,
            Comment = "Test"
        };

        // Act
        var result = (ObjectResult)controller.Create(newEntity).Result;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400); // BadRequest zbog validacije
    }

    [Fact]
    public void Updates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var updatedEntity = new TourReviewDto
        {
            Id = -1,
            Rating = 5,
            Comment = "Izmenjen komentar",
            Images = new List<string> { "new_image.jpg" },
            TourId = -1,
            TouristId = -21,
            ExecutionId = -1, 
            ReviewDate = DateTime.Parse("2024-01-01 10:00:00").ToUniversalTime(),
            CompletedPercentage = 100
        };

        // Act
        var result = ((ObjectResult)controller.Update(updatedEntity).Result)?.Value as TourReviewDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Rating.ShouldBe(5);
        result.Comment.ShouldBe("Izmenjen komentar");

        // Assert - Database
        var storedEntity = dbContext.TourReviews.FirstOrDefault(i => i.Id == -1);
        storedEntity.ShouldNotBeNull();
        storedEntity.Comment.ShouldBe(updatedEntity.Comment);
    }

    [Fact]
    public void Update_fails_invalid_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();

        // Ulogovani smo kao turista2 (-22)
        var controller = CreateController(scope, "-22");

        var updatedEntity = new TourReviewDto
        {
            Id = -1, 
            TourId = -1,
            Rating = 5,
            Comment = "Hakerski pokusaj",
            TouristId = -21,
            ExecutionId = -1, 
            ReviewDate = DateTime.UtcNow,
            CompletedPercentage = 100,
            Images = new List<string>()
        };

        // Act
        var result = (ObjectResult)controller.Update(updatedEntity).Result;

        // Assert
        result.StatusCode.ShouldBe(400);
    }

    private static TourReviewController CreateController(IServiceScope scope, string userId)
    {
        return new TourReviewController(
            scope.ServiceProvider.GetRequiredService<ITourReviewService>(),
            scope.ServiceProvider.GetRequiredService<ITourExecutionService>()
        )
        {
            ControllerContext = BuildContext(userId)
        };
    }
}