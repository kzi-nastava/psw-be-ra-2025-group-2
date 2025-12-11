using Explorer.API.Controllers.Author;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class TourQueryTests : BaseToursIntegrationTest
{
    public TourQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_tours_by_author()
    {
       
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetByAuthor(-11).Result)?.Value as IEnumerable<TourDto>;

        result.ShouldNotBeNull();
        var tourList = result.ToList();
        tourList.Count.ShouldBe(3); 
        tourList.ShouldAllBe(t => t.AuthorId == -11);

        tourList.ShouldContain(t => t.Name == "Fruska Gora Adventure");
        tourList.ShouldContain(t => t.Name == "Novi Sad City Tour");
    }

    [Fact]
    public void Retrieves_tours_by_different_author()
    {
        
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        
        var result = ((ObjectResult)controller.GetByAuthor(-12).Result)?.Value as IEnumerable<TourDto>;

        
        result.ShouldNotBeNull();
        var tourList = result.ToList();
        tourList.Count.ShouldBe(3);
        tourList.ShouldAllBe(t => t.AuthorId == -12);

        tourList.ShouldContain(t => t.Name == "Dunav Kayaking");
        tourList.ShouldContain(t => t.Name == "Stara Planina Extreme");
    }

    [Fact]
    public void Retrieves_empty_list_for_author_without_tours()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetByAuthor(-13).Result)?.Value as IEnumerable<TourDto>;

        result.ShouldNotBeNull();
        result.Count().ShouldBe(0); 
    }

    [Fact]
    public void Retrieved_tour_has_correct_properties()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetByAuthor(-11).Result)?.Value as IEnumerable<TourDto>;

        result.ShouldNotBeNull();
        var tour = result.FirstOrDefault(t => t.Name == "Novi Sad City Tour");

        tour.ShouldNotBeNull();
        tour.Id.ShouldBe(-2);
        tour.Name.ShouldBe("Novi Sad City Tour");
        tour.Description.ShouldBe("Obilazak Petrovaradinske tvrdjave i centra grada");
        tour.Difficulty.ShouldBe(1);
        tour.Status.ShouldBe("Published");
        tour.Price.ShouldBe(15.99m);
        tour.Tags.Count.ShouldBe(3);
        tour.Tags.ShouldContain("grad");
        tour.Tags.ShouldContain("kultura");
        tour.Tags.ShouldContain("istorija");
    }

    private static TourController CreateController(IServiceScope scope)
    {
        return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildContext("-11")
        };
    }
}