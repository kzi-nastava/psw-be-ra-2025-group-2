using Explorer.API.Controllers.Tourist;
using Explorer.Payments.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Public.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class AverageCostPublishedToursTests : BaseToursIntegrationTest
{
    public AverageCostPublishedToursTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Published_tour_contains_average_cost_AC1_AC2_AC4_AC5()
    {
        using var scope = Factory.Services.CreateScope();

        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();

        var publishedTourId = -1;
        var authorId = -11;

        // Arrange: dodamo keypointove da estimator ima šta da klasifikuje
        tourService.AddKeyPoint(publishedTourId, new KeyPointDto
        {
            OrdinalNo = 1,
            Name = "Museum stop",
            Description = "KP for average cost",
            SecretText = "",
            ImageUrl = "",
            Latitude = 45.2671,
            Longitude = 19.8335,
            AuthorId = authorId,
            OsmClass = "tourism",
            OsmType = "museum",
            IsEncounterRequired = false
        });

        tourService.AddKeyPoint(publishedTourId, new KeyPointDto
        {
            OrdinalNo = 2,
            Name = "Cafe stop",
            Description = "KP for average cost",
            SecretText = "",
            ImageUrl = "",
            Latitude = 45.2672,
            Longitude = 19.8336,
            AuthorId = authorId,
            OsmClass = "amenity",
            OsmType = "cafe",
            IsEncounterRequired = false
        });

        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetPublished(page: 1, pageSize: 200);

        // Assert
        var ok = actionResult.Result as OkObjectResult;
        ok.ShouldNotBeNull();

        var pageDto = ok.Value as PagedResultDto<PublishedTourPreviewDto>;
        pageDto.ShouldNotBeNull();

        var tour = pageDto.Results.FirstOrDefault(t => t.Id == publishedTourId);
        tour.ShouldNotBeNull();

        tour.AverageCost.ShouldNotBeNull();
        tour.AverageCost!.Breakdown.ShouldNotBeNull();
        tour.AverageCost.Disclaimer.ShouldNotBeNullOrWhiteSpace();

        // AC2: postoji razrada
        var b = tour.AverageCost.Breakdown!;
        // (ne mora da bude > 0, ali je bolje da bar nešto bude >0 ako hoćeš strože)
        (b.Tickets + b.Transport + b.FoodAndDrink + b.Other).ShouldBe(tour.AverageCost.TotalPerPerson);
    }

    private static TourController CreateController(IServiceScope scope)
        => new TourController(
            scope.ServiceProvider.GetRequiredService<ITourService>(),
            scope.ServiceProvider.GetRequiredService<IPaymentRecordService>(),
            scope.ServiceProvider.GetRequiredService<ITourExecutionService>(),
            scope.ServiceProvider.GetRequiredService<ITourPurchaseTokenRepository>()
        );
}
