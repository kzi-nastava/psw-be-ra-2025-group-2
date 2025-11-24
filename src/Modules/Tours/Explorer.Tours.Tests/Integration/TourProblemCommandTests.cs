using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration;

[Collection("Sequential")]
public class TourProblemCommandTests : BaseToursIntegrationTest
{
    public TourProblemCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_tour_problem_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new CreateTourProblemDto
        {
            TourId = -5,   
            Category = "Technical",
            Priority = "High",
            Description = "Test problem from integration tests"
        };

        // Act
        var result = (ObjectResult)controller.Create(dto).Result!;
        var created = result.Value as TourProblemDto;

        // Assert
        created.ShouldNotBeNull();
        created!.Id.ShouldNotBe(0);
        created.TourId.ShouldBe(-5);
        created.Category.ShouldBe("Technical");
        created.Priority.ShouldBe("High");
    }

    [Fact]
    public void Gets_problems_for_creator()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = (ObjectResult)controller.GetForCreator().Result!;
        var problems = result.Value as List<TourProblemDto>;

        // Assert
        problems.ShouldNotBeNull();
        problems!.Count.ShouldBeGreaterThan(0);
    }

    private static TourProblemController CreateController(IServiceScope scope)
    {
        return new TourProblemController(scope.ServiceProvider.GetRequiredService<ITourProblemService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
