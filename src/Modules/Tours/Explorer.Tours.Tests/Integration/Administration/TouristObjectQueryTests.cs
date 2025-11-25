using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class TouristObjectQueryTests : BaseToursIntegrationTest
{
    public TouristObjectQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_all()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetAll(0, 0).Result)?.Value as PagedResult<TouristObjectDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBe(5); 
        result.TotalCount.ShouldBe(5);
    }

    private static TouristObjectController CreateController(IServiceScope scope)
    {
        return new TouristObjectController(scope.ServiceProvider.GetRequiredService<ITouristObjectService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
