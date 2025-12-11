using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Public.Execution;
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
        // Tražimo recenzije za turu sa ID -1 (gde smo ubacili 2 recenzije)
        var result = ((ObjectResult)controller.GetByTourId(-1, 1, 10).Result)?.Value as PagedResult<TourReviewDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBe(2); // Očekujemo 2 recenzije za turu -1
        result.TotalCount.ShouldBe(2);
    }

    private static TourReviewController CreateController(IServiceScope scope)
    {
        return new TourReviewController(
            scope.ServiceProvider.GetRequiredService<ITourReviewService>(),
            scope.ServiceProvider.GetRequiredService<ITourExecutionService>()
        )
        {
            ControllerContext = BuildContext("-21")
        };
    }
}