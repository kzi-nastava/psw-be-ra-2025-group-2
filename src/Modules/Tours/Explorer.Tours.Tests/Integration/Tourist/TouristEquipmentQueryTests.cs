
using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TouristEquipmentQueryTests : BaseToursIntegrationTest
{
    public TouristEquipmentQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_tourist_equipment()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.Get(-1).Result)?.Value as TouristEquipmentDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Equipment.ShouldNotBeNull();
        result.Equipment.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Retrieves_all_equipment()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetAllEquipment(0, 0).Result)?.Value
                     as PagedResult<EquipmentDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBe(3);
        result.TotalCount.ShouldBe(3);
    }

    private static TouristEquipmentController CreateController(IServiceScope scope)
    {
        return new TouristEquipmentController(
            scope.ServiceProvider.GetRequiredService<ITouristEquipmentService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
