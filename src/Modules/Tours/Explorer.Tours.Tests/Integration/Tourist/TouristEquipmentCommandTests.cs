using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TouristEquipmentCommandTests : BaseToursIntegrationTest
{
    public TouristEquipmentCommandTests(ToursTestFactory factory) : base(factory)
    {
    }

    private static TouristEquipmentController CreateController(IServiceScope scope)
    {
        return new TouristEquipmentController(
            scope.ServiceProvider.GetRequiredService<ITouristEquipmentService>())
        {
            ControllerContext = BuildContext("-1")   // turista -1
        };
    }

    // ----------------------------------------------------------------------
    // CREATE SUCCESS
    // ----------------------------------------------------------------------
    [Fact]
    public void Create_succeeds()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new TouristEquipmentDto
        {
            TouristId = -10,
            Equipment = new List<int> { -1, -2 }     // valid equipment
        };

        var result = ((ObjectResult)controller.Create(dto).Result)?.Value as TouristEquipmentDto;

        result.ShouldNotBeNull();
        result.TouristId.ShouldBe(-10);
        result.Equipment.Count.ShouldBe(2);
        result.Equipment.ShouldContain(-1);
        result.Equipment.ShouldContain(-2);
    }

    // ----------------------------------------------------------------------
    // CREATE FAIL – INVALID EQUIPMENT IDS
    // ----------------------------------------------------------------------
    [Fact]
    public void Create_fails_invalid_equipment()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new TouristEquipmentDto
        {
            TouristId = -15,
            Equipment = new List<int> { 9999 }    // ne postoji
        };

        Should.Throw<Exception>(() => controller.Create(dto));
    }

    // ----------------------------------------------------------------------
    // UPDATE SUCCESS
    // ----------------------------------------------------------------------
    [Fact]
    public void Update_succeeds()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // postoji u c-touristequipment.sql → ID = -1
        var dto = new TouristEquipmentDto
        {
            Id = -1,
            TouristId = -1,
            Equipment = new List<int> { -1, -3 }   // promenjeno
        };

        var result = ((ObjectResult)controller.Update(dto).Result)?.Value as TouristEquipmentDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Equipment.Count.ShouldBe(2);
        result.Equipment.ShouldContain(-1);
        result.Equipment.ShouldContain(-3);
    }

    // ----------------------------------------------------------------------
    // UPDATE FAIL – NONEXISTENT ID
    // ----------------------------------------------------------------------
    [Fact]
    public void Update_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new TouristEquipmentDto
        {
            Id = -9999,
            TouristId = -9999,
            Equipment = new List<int>()
        };

        Should.Throw<Exception>(() => controller.Update(dto));
    }

    // ----------------------------------------------------------------------
    // UPDATE FAIL – INVALID EQUIPMENT IDS
    // ----------------------------------------------------------------------
    [Fact]
    public void Update_fails_invalid_equipment()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new TouristEquipmentDto
        {
            Id = -1,
            TouristId = -1,
            Equipment = new List<int> { 12345 }     // ne postoji equipment
        };

        Should.Throw<Exception>(() => controller.Update(dto));
    }
}
