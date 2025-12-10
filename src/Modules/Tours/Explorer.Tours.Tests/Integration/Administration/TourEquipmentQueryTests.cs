using System.Collections.Generic;
using System.Linq;
using Explorer.API.Controllers.Author;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class TourEquipmentQueryTests : BaseToursIntegrationTest
{
    public TourEquipmentQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void GetEquipmentForTour_returns_all_equipment_for_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");   // authorId iz seed-a
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tourId = -11;

        // Act
        var actionResult = controller.GetEquipmentForTour(tourId);
        var okResult = actionResult.Result as OkObjectResult;
        var result = okResult?.Value as List<TourEquipmentItemDto>;

        // Assert - Response
        result.ShouldNotBeNull();

        // U bazi imamo 3 komada opreme: -1, -2, -3
        result!.Count.ShouldBe(3);

        var expectedEquipmentIds = dbContext.Equipment
            .Select(e => e.Id)
            .ToList();

        // ista lista kao u bazi
        result.Select(e => e.Id).OrderBy(x => x)
            .ShouldBe(expectedEquipmentIds.OrderBy(x => x));
    }


    [Fact]
    public void GetAllEquipmentForAuthor_returns_all_equipment_for_author()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var authorId = "-11"; 
        var controller = CreateController(scope, authorId);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var actionResult = controller.GetAllEquipmentForAuthor();
        var okResult = actionResult.Result as OkObjectResult;
        var result = okResult?.Value as List<TourEquipmentItemDto>;

        // Assert
        result.ShouldNotBeNull();

        // očekujemo da vrati svu opremu iz baze
        var expectedEquipmentIds = dbContext.Equipment
            .Select(e => e.Id)
            .ToList();

        expectedEquipmentIds.Count.ShouldBe(3);           // po seed-u imaš 3
        result!.Count.ShouldBe(expectedEquipmentIds.Count);

        // jedinstveni ID-jevi
        result.Select(e => e.Id).Distinct().Count()
            .ShouldBe(result.Count);

        // isti skup ID-jeva kao u bazi
        result.Select(e => e.Id).OrderBy(x => x)
            .ShouldBe(expectedEquipmentIds.OrderBy(x => x));
    }


    private static TourController CreateController(IServiceScope scope, string authorId)
    {
        return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildContext(authorId)
        };
    }
}
