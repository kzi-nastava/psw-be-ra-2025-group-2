/*using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Dodato za .Include ako zatreba proveru baze
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
        var controller = CreateController(scope, "-21"); // Koristim -21 jer on obično ima seed-ovan Execution
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var newEntity = new TourReviewDto
        {
            TourId = -1,
            ExecutionId = -1, // Nije presudno jer servis sam traži execution, ali neka stoji
            Rating = 5,
            Comment = "Odlična tura, preporučujem!",
            Images = new List<string> { "new_image.jpg" },
            TouristId = -21,
            ReviewDate = DateTime.UtcNow,
            CompletedPercentage = 100 // Servis će ovo preračunati, ali šaljemo ga
        };

        // Act
        // OVDE JE PROMENA: Zovemo RateTour umesto Create
        var response = ((ObjectResult)controller.RateTour(newEntity).Result)?.Value as TourReviewDto;

        // Assert - Response
        response.ShouldNotBeNull();
        response.TourId.ShouldBe(-1);
        response.Comment.ShouldBe(newEntity.Comment);
        response.Rating.ShouldBe(newEntity.Rating);

        // Assert - Database
        // Proveravamo da li je recenzija upisana u listu recenzija Ture
        var tourInDb = dbContext.Tours
            .Include(t => t.Reviews)
            .FirstOrDefault(t => t.Id == -1);

        tourInDb.ShouldNotBeNull();
        var reviewInDb = tourInDb.Reviews.FirstOrDefault(r => r.Comment == "Odlična tura, preporučujem!");
        reviewInDb.ShouldNotBeNull();
        reviewInDb.Rating.ShouldBe(5);
    }

    [Fact]
    public void Create_fails_invalid_rating()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");
        var newEntity = new TourReviewDto
        {
            TourId = -1,
            ExecutionId = -1,
            Rating = 10, // Neispravna ocena
            Comment = "Test"
        };

        // Act
        // Zovemo RateTour
        var result = (ObjectResult)controller.RateTour(newEntity).Result;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400); // BadRequest zbog validacije
    }

    // NAPOMENA: Update testove smo izbacili jer trenutno nemamo endpoint za Update recenzije
    // unutar TourController-a (samo RateTour). Ako dodaš Update endpoint, vrati testove.

    private static TourController CreateController(IServiceScope scope, string userId)
    {
        return new TourController(
            scope.ServiceProvider.GetRequiredService<ITourService>()
        )
        {
            ControllerContext = BuildContext(userId)
        };
    }
}*/