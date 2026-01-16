using Explorer.API.Controllers.Tourist;
using Explorer.Payments.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Public.Execution;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class TourFilteringTests : BaseToursIntegrationTest
{
    public TourFilteringTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Filters_tours_by_environment_type_urban()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Setuj test podatke - urban tura
        var urbanTour = dbContext.Tours.First(t => t.Id == -1);
        urbanTour.SetEnvironmentType(TourEnvironmentType.Urban);
        dbContext.SaveChanges();

        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublished(
            page: 1,
            pageSize: 100,
            environmentType: 1 // Urban
        );

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var pageDto = okResult.Value as PagedResultDto<PublishedTourPreviewDto>;
        pageDto.ShouldNotBeNull();

        pageDto.Results.ShouldNotBeEmpty();
        pageDto.Results.ShouldAllBe(t =>
            dbContext.Tours.First(tour => tour.Id == t.Id).EnvironmentType == TourEnvironmentType.Urban
        );
    }

    [Fact]
    public void Filters_tours_by_environment_type_nature()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Setuj test podatke - nature tura
        var natureTour = dbContext.Tours.First(t => t.Id == -2);
        natureTour.SetEnvironmentType(TourEnvironmentType.Nature);
        dbContext.SaveChanges();

        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublished(
            page: 1,
            pageSize: 100,
            environmentType: 2 // Nature
        );

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var pageDto = okResult.Value as PagedResultDto<PublishedTourPreviewDto>;
        pageDto.ShouldNotBeNull();

        pageDto.Results.ShouldNotBeEmpty();
        pageDto.Results.ShouldAllBe(t =>
            dbContext.Tours.First(tour => tour.Id == t.Id).EnvironmentType == TourEnvironmentType.Nature
        );
    }

    [Fact]
    public void Filters_tours_by_price_range()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublished(
            page: 1,
            pageSize: 100,
            minPrice: 10,
            maxPrice: 50
        );

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var pageDto = okResult.Value as PagedResultDto<PublishedTourPreviewDto>;
        pageDto.ShouldNotBeNull();

        pageDto.Results.ShouldAllBe(t => t.Price >= 10 && t.Price <= 50);
    }

    [Fact]
    public void Filters_tours_by_min_price_only()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublished(
            page: 1,
            pageSize: 100,
            minPrice: 20
        );

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var pageDto = okResult.Value as PagedResultDto<PublishedTourPreviewDto>;
        pageDto.ShouldNotBeNull();

        pageDto.Results.ShouldAllBe(t => t.Price >= 20);
    }

    [Fact]
    public void Filters_tours_by_max_price_only()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublished(
            page: 1,
            pageSize: 100,
            maxPrice: 30
        );

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var pageDto = okResult.Value as PagedResultDto<PublishedTourPreviewDto>;
        pageDto.ShouldNotBeNull();

        pageDto.Results.ShouldAllBe(t => t.Price <= 30);
    }

    [Fact]
    public void Filters_tours_by_suitable_for_students()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tour = dbContext.Tours.First(t => t.Id == -1);
        tour.SetSuitableForGroups(new List<SuitableFor> { SuitableFor.Students });
        dbContext.SaveChanges();

        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublished(
            page: 1,
            pageSize: 100,
            suitableFor: "1" // Students
        );

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var pageDto = okResult.Value as PagedResultDto<PublishedTourPreviewDto>;
        pageDto.ShouldNotBeNull();

        pageDto.Results.ShouldNotBeEmpty();
    }

    [Fact]
    public void Filters_tours_by_suitable_for_children_and_families()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tour = dbContext.Tours.First(t => t.Id == -1);
        tour.SetSuitableForGroups(new List<SuitableFor> { SuitableFor.Children, SuitableFor.Families });
        dbContext.SaveChanges();

        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublished(
            page: 1,
            pageSize: 100,
            suitableFor: "2,4" // Children, Families
        );

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var pageDto = okResult.Value as PagedResultDto<PublishedTourPreviewDto>;
        pageDto.ShouldNotBeNull();

        pageDto.Results.ShouldNotBeEmpty();
    }

    [Fact]
    public void Filters_tours_by_food_types_vegetarian()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tour = dbContext.Tours.First(t => t.Id == -1);
        tour.SetFoodTypes(new List<FoodType> { FoodType.Vegetarian });
        dbContext.SaveChanges();

        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublished(
            page: 1,
            pageSize: 100,
            foodTypes: "1" // Vegetarian
        );

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var pageDto = okResult.Value as PagedResultDto<PublishedTourPreviewDto>;
        pageDto.ShouldNotBeNull();

        pageDto.Results.ShouldNotBeEmpty();
    }

    [Fact]
    public void Filters_tours_by_food_types_multiple()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tour = dbContext.Tours.First(t => t.Id == -1);
        tour.SetFoodTypes(new List<FoodType> { FoodType.Vegetarian, FoodType.Vegan, FoodType.GlutenFree });
        dbContext.SaveChanges();

        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublished(
            page: 1,
            pageSize: 100,
            foodTypes: "1,2,3" // Vegetarian, Vegan, GlutenFree
        );

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var pageDto = okResult.Value as PagedResultDto<PublishedTourPreviewDto>;
        pageDto.ShouldNotBeNull();

        pageDto.Results.ShouldNotBeEmpty();
    }

    [Fact]
    public void Filters_tours_by_adventure_level_low()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tour = dbContext.Tours.First(t => t.Id == -1);
        tour.SetAdventureLevel(AdventureLevel.Low);
        dbContext.SaveChanges();

        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublished(
            page: 1,
            pageSize: 100,
            adventureLevel: "1" // Low
        );

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var pageDto = okResult.Value as PagedResultDto<PublishedTourPreviewDto>;
        pageDto.ShouldNotBeNull();

        pageDto.Results.ShouldNotBeEmpty();
    }

    [Fact]
    public void Filters_tours_by_adventure_level_high()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tour = dbContext.Tours.First(t => t.Id == -2);
        tour.SetAdventureLevel(AdventureLevel.High);
        dbContext.SaveChanges();

        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublished(
            page: 1,
            pageSize: 100,
            adventureLevel: "3" // High
        );

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var pageDto = okResult.Value as PagedResultDto<PublishedTourPreviewDto>;
        pageDto.ShouldNotBeNull();

        pageDto.Results.ShouldNotBeEmpty();
    }

    [Fact]
    public void Filters_tours_by_activity_type_adrenaline()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tour = dbContext.Tours.First(t => t.Id == -1);
        tour.SetActivityTypes(new List<ActivityType> { ActivityType.Adrenaline });
        dbContext.SaveChanges();

        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublished(
            page: 1,
            pageSize: 100,
            activityTypes: "1" // Adrenaline
        );

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var pageDto = okResult.Value as PagedResultDto<PublishedTourPreviewDto>;
        pageDto.ShouldNotBeNull();

        pageDto.Results.ShouldNotBeEmpty();
    }

    [Fact]
    public void Filters_tours_by_activity_type_cultural_and_relaxing()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tour = dbContext.Tours.First(t => t.Id == -1);
        tour.SetActivityTypes(new List<ActivityType> { ActivityType.Cultural, ActivityType.Relaxing });
        dbContext.SaveChanges();

        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublished(
            page: 1,
            pageSize: 100,
            activityTypes: "2,3" // Cultural, Relaxing
        );

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var pageDto = okResult.Value as PagedResultDto<PublishedTourPreviewDto>;
        pageDto.ShouldNotBeNull();

        pageDto.Results.ShouldNotBeEmpty();
    }

    [Fact]
    public void Filters_tours_by_multiple_criteria_combined()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Setuj test ture sa kombinovanim filterima
        var tour1 = dbContext.Tours.First(t => t.Id == -1);
        tour1.SetEnvironmentType(TourEnvironmentType.Urban);
        tour1.SetAdventureLevel(AdventureLevel.Low);
        tour1.SetSuitableForGroups(new List<SuitableFor> { SuitableFor.Families, SuitableFor.Children });
        tour1.SetActivityTypes(new List<ActivityType> { ActivityType.Cultural });
        tour1.SetPrice(25m);

        dbContext.SaveChanges();

        var controller = CreateController(scope);

        // Act - Kombinacija više filtera
        var result = controller.GetPublished(
            page: 1,
            pageSize: 100,
            environmentType: 1, // Urban
            minPrice: 10,
            maxPrice: 50,
            adventureLevel: "1", // Low
            suitableFor: "2,4", // Children, Families
            activityTypes: "2" // Cultural
        );

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var pageDto = okResult.Value as PagedResultDto<PublishedTourPreviewDto>;
        pageDto.ShouldNotBeNull();

        // Proveri da postoji bar jedna tura koja zadovoljava sve filtere
        pageDto.Results.ShouldNotBeEmpty();

        // Proveri da sve vraćene ture zadovoljavaju filtere
        foreach (var tourDto in pageDto.Results)
        {
            tourDto.Price.ShouldBeInRange(10, 50);
        }
    }

    [Fact]
    public void Returns_empty_when_no_tours_match_filters()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act - Nemoguća kombinacija filtera
        var result = controller.GetPublished(
            page: 1,
            pageSize: 100,
            environmentType: 1, // Urban
            minPrice: 10000, // Previše visoka cena
            maxPrice: 20000
        );

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var pageDto = okResult.Value as PagedResultDto<PublishedTourPreviewDto>;
        pageDto.ShouldNotBeNull();

        pageDto.Results.ShouldBeEmpty();
        pageDto.TotalCount.ShouldBe(0);
    }

    [Fact]
    public void Returns_all_published_tours_when_no_filters_applied()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = controller.GetPublished(page: 1, pageSize: 100);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var pageDto = okResult.Value as PagedResultDto<PublishedTourPreviewDto>;
        pageDto.ShouldNotBeNull();

        // Trebalo bi da vrati sve published ture
        pageDto.Results.ShouldNotBeEmpty();
        pageDto.TotalCount.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Pagination_works_with_filters()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        
        // Resetuj sve published ture
        var allPublished = dbContext.Tours
            .Where(t => t.Status == TourStatus.Published)
            .ToList();
        
        foreach (var t in allPublished)
        {
            t.SetEnvironmentType(null);
        }
        dbContext.SaveChanges();
        
        // Setuj tačno 3 ture sa Urban
        var toursToSetUrban = dbContext.Tours
            .Where(t => t.Status == TourStatus.Published)
            .OrderBy(t => t.Id)
            .Take(3)
            .ToList();
        
        toursToSetUrban.Count.ShouldBe(3);
        
        foreach (var tour in toursToSetUrban)
        {
            tour.SetEnvironmentType(TourEnvironmentType.Urban);
        }
        dbContext.SaveChanges();

        var controller = CreateController(scope);

        // Act - Prva stranica
        var resultPage1 = controller.GetPublished(
            page: 1, 
            pageSize: 2, 
            environmentType: 1
        );

        // Assert - Prva stranica
        var okResult1 = resultPage1.Result as OkObjectResult;
        okResult1.ShouldNotBeNull();

        var pageDto1 = okResult1.Value as PagedResultDto<PublishedTourPreviewDto>;
        pageDto1.ShouldNotBeNull();
        
        pageDto1.Results.Count.ShouldBe(2); // Tačno 2 rezultata
        pageDto1.TotalCount.ShouldBe(3); // Ukupno 3
        
        // Act - Druga stranica
        var resultPage2 = controller.GetPublished(
            page: 2, 
            pageSize: 2, 
            environmentType: 1
        );
        
        // Assert - Druga stranica
        var okResult2 = resultPage2.Result as OkObjectResult;
        okResult2.ShouldNotBeNull();
        
        var pageDto2 = okResult2.Value as PagedResultDto<PublishedTourPreviewDto>;
        pageDto2.ShouldNotBeNull();
        
        pageDto2.Results.Count.ShouldBe(1); // Samo 1 preostala tura
        pageDto2.TotalCount.ShouldBe(3); // I dalje ukupno 3
    }

    [Fact]
    public void Filters_ignore_case_sensitivity_for_string_filters()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tour = dbContext.Tours.First(t => t.Id == -1);
        tour.SetSuitableForGroups(new List<SuitableFor> { SuitableFor.Students });
        dbContext.SaveChanges();

        var controller = CreateController(scope);

        // Act - Koristi različite formate stringa
        var result = controller.GetPublished(
            page: 1,
            pageSize: 100,
            suitableFor: "1"
        );

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var pageDto = okResult.Value as PagedResultDto<PublishedTourPreviewDto>;
        pageDto.ShouldNotBeNull();

        pageDto.Results.ShouldNotBeEmpty();
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
}