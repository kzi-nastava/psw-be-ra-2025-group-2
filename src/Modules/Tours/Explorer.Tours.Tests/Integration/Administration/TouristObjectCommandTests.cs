using Explorer.API.Controllers.Administrator.Administration;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class TouristObjectCommandTests : BaseToursIntegrationTest
{
    public TouristObjectCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var newEntity = new TouristObjectDto
        {
            Name = "New Tourist Spot",
            Latitude = 45.10,
            Longitude = 19.88,
            Category = Explorer.Tours.Core.Domain.TouristObjectCategory.Other.ToString() 
        };

        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as TouristObjectDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);
        result.Category.ShouldBe(newEntity.Category);

        // Assert - Database
        var storedEntity = dbContext.TouristObject.FirstOrDefault(i => i.Name == newEntity.Name);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
        storedEntity.Category.ToString().ShouldBe(newEntity.Category); 
    }

    [Fact]
    public void Updates()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var updatedEntity = new TouristObjectDto
        {
            Id = -1,
            Name = "Updated Spot",
            Latitude = 45.20,
            Longitude = 19.90,
            Category = Explorer.Tours.Core.Domain.TouristObjectCategory.Restaurant.ToString() 
        };

        var result = ((ObjectResult)controller.Update(updatedEntity).Result)?.Value as TouristObjectDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Name.ShouldBe(updatedEntity.Name);
        result.Category.ShouldBe(updatedEntity.Category);

        // Assert - Database
        var storedEntity = dbContext.TouristObject.FirstOrDefault(i => i.Id == -1);
        storedEntity.ShouldNotBeNull();
        storedEntity.Name.ShouldBe(updatedEntity.Name);
        storedEntity.Category.ToString().ShouldBe(updatedEntity.Category); 
    }

    [Fact]
    public void Deletes()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var result = (OkResult)controller.Delete(-2);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedEntity = dbContext.TouristObject.FirstOrDefault(i => i.Id == -2);
        storedEntity.ShouldBeNull();
    }

    private static TouristObjectController CreateController(IServiceScope scope)
    {
        return new TouristObjectController(scope.ServiceProvider.GetRequiredService<ITouristObjectService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
