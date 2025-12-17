using Explorer.API.Controllers.Author;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class TourLengthCommandTests : BaseToursIntegrationTest
{
    public TourLengthCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Updates_length_km()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var updateDto = new UpdateTourDto
        {
            Name = "Tour Updated Length",
            Description = "Updated Description",
            Difficulty = 2,
            LengthKm = 12.72m
        };

        var result = ((ObjectResult)controller.Update(-11, updateDto).Result)?.Value as TourDto;

        result.ShouldNotBeNull();
        result.LengthKm.ShouldBe(12.72m);

        var stored = dbContext.Tours.First(t => t.Id == -11);
        stored.LengthKm.ShouldBe(12.72m);
    }

    [Fact]
    public void Updates_length_km_null()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var updateDto = new UpdateTourDto
        {
            Name = "Tour Null Length",
            Description = "Test null",
            Difficulty = 3,
            LengthKm = null
        };

        var result = ((ObjectResult)controller.Update(-11, updateDto).Result)?.Value as TourDto;

        result.ShouldNotBeNull();
        result.LengthKm.ShouldBeNull();

        var stored = dbContext.Tours.First(t => t.Id == -11);
        stored.LengthKm.ShouldBeNull();
    }

    

    private static TourController CreateController(IServiceScope scope, string userId)
    {
        return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildContext(userId)
        };
    }
}
