using Explorer.API.Controllers.Tourist;
using Explorer.Payments.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Public.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourPlannerDetailsAverageCostTests : BaseToursIntegrationTest
{
    public TourPlannerDetailsAverageCostTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Planner_details_returns_average_cost_AC()
    {
        using var scope = Factory.Services.CreateScope();

        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();

        // 1) Napravi novu turu kroz servis (da se estimator pozove i snimi AverageCost u bazu)
        var created = tourService.Create(new CreateTourDto
        {
            Name = "Avg cost test tour",
            Description = "tour for avg cost test",
            Difficulty = 3,
            AuthorId = 1,          // BITNO: domain ti traži authorId > 0
            Price = 0,
            Tags = new() { "test" },
            KeyPoints = new()
            {
                new KeyPointDto
                {
                    OrdinalNo = 1,
                    Name = "KP1",
                    Description = "desc",
                    ImageUrl = "http://img",
                    Latitude = 45.2671,
                    Longitude = 19.8335,
                    AuthorId = 1,
                    OsmClass = "amenity",
                    OsmType = "cafe"
                },
                new KeyPointDto
                {
                    OrdinalNo = 2,
                    Name = "KP2",
                    Description = "desc2",
                    ImageUrl = "http://img2",
                    Latitude = 45.2675,
                    Longitude = 19.8340,
                    AuthorId = 1,
                    OsmClass = "tourism",
                    OsmType = "museum"
                }
            }
        });

        // (opciono) Update da bi LengthKm bio != 0 pa da transport dobije smisla
        tourService.Update(created.Id, new UpdateTourDto
        {
            Name = created.Name,
            Description = created.Description,
            Difficulty = created.Difficulty,
            Price = created.Price,
            Tags = created.Tags,
            LengthKm = 10m,
            KeyPoints = new(),   // Update ne menja keypoint-e u tvojoj implementaciji
            Durations = new()
        });

        var controller = CreateController(scope);

        // 2) Act: planner-details
        var actionResult = controller.GetTourInfo(created.Id);

        // 3) Assert
        var ok = actionResult.Result as OkObjectResult;
        ok.ShouldNotBeNull();

        var dto = ok.Value as FullTourInfoDto;
        dto.ShouldNotBeNull();

        dto.AverageCost.ShouldNotBeNull();
        dto.AverageCost!.TotalPerPerson.ShouldBeGreaterThanOrEqualTo(0);

        dto.AverageCost.Currency.ShouldNotBeNullOrWhiteSpace();
        dto.AverageCost.Disclaimer.ShouldNotBeNullOrWhiteSpace();

        dto.AverageCost.Breakdown.ShouldNotBeNull();
        dto.AverageCost.Breakdown!.Tickets.ShouldBeGreaterThanOrEqualTo(0);
        dto.AverageCost.Breakdown.Transport.ShouldBeGreaterThanOrEqualTo(0);
        dto.AverageCost.Breakdown.FoodAndDrink.ShouldBeGreaterThanOrEqualTo(0);
        dto.AverageCost.Breakdown.Other.ShouldBeGreaterThanOrEqualTo(0);
    }

    private static TourController CreateController(IServiceScope scope)
    {
        return new TourController(
            scope.ServiceProvider.GetRequiredService<ITourService>(),
            scope.ServiceProvider.GetRequiredService<IPaymentRecordService>(),
            scope.ServiceProvider.GetRequiredService<ITourExecutionService>(),
            scope.ServiceProvider.GetRequiredService<ITourPurchaseTokenRepository>()
        );
    }
}
