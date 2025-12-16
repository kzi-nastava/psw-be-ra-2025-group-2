/*using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourReviewQueryTests : BaseToursIntegrationTest
{
    public TourReviewQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_all_for_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        // OVDE JE PROMENA: Umesto GetByTourId, zovemo GetPublished jer on vraća ture SA recenzijama
        var response = ((OkObjectResult)controller.GetPublished().Result)?.Value as List<PublishedTourPreviewDto>;

        // Assert
        response.ShouldNotBeNull();
        response.ShouldNotBeEmpty();

        // Tražimo turu -1 koja ima seed-ovane recenzije
        var targetTour = response.FirstOrDefault(t => t.Id == -1);
        targetTour.ShouldNotBeNull();

        // Proveravamo da li ta tura ima recenzije (kao što je stari test proveravao listu)
        targetTour.Reviews.ShouldNotBeNull();
        targetTour.Reviews.Count.ShouldBeGreaterThanOrEqualTo(2); // Očekujemo bar 2 recenzije iz seed-a
        targetTour.AverageRating.ShouldBeGreaterThan(0);
    }

    private static TourController CreateController(IServiceScope scope)
    {
        return new TourController(
            scope.ServiceProvider.GetRequiredService<ITourService>()
        )
        {
            ControllerContext = BuildContext("-21")
        };
    }
}*/