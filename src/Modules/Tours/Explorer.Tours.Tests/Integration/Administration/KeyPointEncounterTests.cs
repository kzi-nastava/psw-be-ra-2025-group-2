using Explorer.API.Controllers.Author;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class KeyPointEncounterTests : BaseToursIntegrationTest
{
    public KeyPointEncounterTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public async Task Successfully_links_encounter_to_keypoint()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tourId = -1;
        var keyPointOrdinal = 1;
        var updatedKeyPoint = new KeyPointDto
        {
            Id = -1,
            OrdinalNo = keyPointOrdinal,
            Name = "Ažurirana tačka sa izazovom",
            Description = "Opis",
            Latitude = 45.245,
            Longitude = 19.85,
            ImageUrl = "http://image.com",
            EncounterId = -10,
            IsEncounterRequired = true
        };

        var actionResult = await controller.UpdateKeyPoint(tourId, keyPointOrdinal, updatedKeyPoint);
        var result = (actionResult.Result as ObjectResult)?.Value as KeyPointDto;

        result.ShouldNotBeNull();
        result.EncounterId.ShouldBe(-10);
        result.IsEncounterRequired.ShouldBeTrue();
    }

    [Fact]
    public async Task Fails_to_mark_encounter_required_without_encounter_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var tourId = -1;
        var keyPointOrdinal = 1;
        var invalidKeyPoint = new KeyPointDto
        {
            Id = -1,
            OrdinalNo = keyPointOrdinal,
            Name = "Nevalidna tačka",
            Description = "Opis",
            Latitude = 45.245,
            Longitude = 19.85,
            ImageUrl = "http://image.com",
            EncounterId = null,
            IsEncounterRequired = true
        };

        // Act
        var actionResult = await controller.UpdateKeyPoint(tourId, keyPointOrdinal, invalidKeyPoint);

        // Assert
        // Budući da tvoj kontroler trenutno ne hvata ArgumentException posebno,
        // on upada u catch(Exception) i vraća StatusCode(500).
        var result = actionResult.Result as ObjectResult;

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(500);

        // Proveravamo poruku koju tvoj kontroler vraća u catch bloku (unutar anonimnog objekta)
        var responseValue = result.Value.ToString();
        responseValue.ShouldContain("An unexpected error occurred. Please try again.");
    }

    [Fact]
    public async Task Successfully_updates_keypoint_to_remove_encounter()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var tourId = -1;
        var keyPointOrdinal = 1;
        var updatedKeyPoint = new KeyPointDto
        {
            Id = -1,
            OrdinalNo = keyPointOrdinal,
            Name = "Tačka bez izazova",
            Description = "Opis",
            Latitude = 45.245,
            Longitude = 19.85,
            ImageUrl = "http://image.com",
            EncounterId = null,
            IsEncounterRequired = false
        };

        var actionResult = await controller.UpdateKeyPoint(tourId, keyPointOrdinal, updatedKeyPoint);
        var result = (actionResult.Result as ObjectResult)?.Value as KeyPointDto;

        result.ShouldNotBeNull();
        result.EncounterId.ShouldBeNull();
        result.IsEncounterRequired.ShouldBeFalse();
    }

    private static TourController CreateController(IServiceScope scope)
    {
        return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}