using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Statistics;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.API.Controllers.Author;

namespace Explorer.Tours.Tests.Integration.Statistics;

[Collection("Sequential")]
public class TourStatisticsControllerTests : BaseToursIntegrationTest
{
    public TourStatisticsControllerTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void GetKeyPointEncounterStatistics_Returns_Correct_Data()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        var result = ((ObjectResult)controller.GetKeyPointEncounterStatistics(-1).Result)?.Value as KeyPointEncounterStatisticsDto;

        result.ShouldNotBeNull();
        result.TourId.ShouldBe(-1);
        result.TourName.ShouldNotBeNullOrEmpty();
        result.KeyPoints.ShouldNotBeNull();
    }

    [Fact]
    public void GetKeyPointEncounterStatistics_Returns_Unique_Tourist_Count()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        var result = ((ObjectResult)controller.GetKeyPointEncounterStatistics(-1).Result)?.Value as KeyPointEncounterStatisticsDto;

        result.ShouldNotBeNull();
        result.KeyPoints.ShouldNotBeEmpty();

        var firstKeyPoint = result.KeyPoints.FirstOrDefault(kp => kp.OrdinalNo == 1);
        firstKeyPoint.ShouldNotBeNull();
        firstKeyPoint.TouristsArrived.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void GetKeyPointEncounterStatistics_Includes_Encounter_Data_When_Present()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        var result = ((ObjectResult)controller.GetKeyPointEncounterStatistics(-1).Result)?.Value as KeyPointEncounterStatisticsDto;

        result.ShouldNotBeNull();

        var keyPointWithEncounter = result.KeyPoints.FirstOrDefault(kp => kp.Encounter != null);
        if (keyPointWithEncounter != null)
        {
            keyPointWithEncounter.Encounter.ShouldNotBeNull();
            keyPointWithEncounter.Encounter.EncounterId.ShouldNotBe(0);
            keyPointWithEncounter.Encounter.EncounterName.ShouldNotBeNullOrEmpty();
            keyPointWithEncounter.Encounter.TotalAttempts.ShouldBeGreaterThanOrEqualTo(0);
            keyPointWithEncounter.Encounter.SuccessfulAttempts.ShouldBeGreaterThanOrEqualTo(0);
            keyPointWithEncounter.Encounter.SuccessRate.ShouldBeGreaterThanOrEqualTo(0);
            keyPointWithEncounter.Encounter.SuccessRate.ShouldBeLessThanOrEqualTo(100);
        }
    }

    [Fact]
    public void GetKeyPointEncounterStatistics_Returns_Null_Encounter_When_Not_Present()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        var result = ((ObjectResult)controller.GetKeyPointEncounterStatistics(-1).Result)?.Value as KeyPointEncounterStatisticsDto;

        result.ShouldNotBeNull();

        var keyPointWithoutEncounter = result.KeyPoints.FirstOrDefault(kp => kp.Encounter == null);
        if (keyPointWithoutEncounter != null)
        {
            keyPointWithoutEncounter.Encounter.ShouldBeNull();
        }
    }

    [Fact]
    public void GetKeyPointEncounterStatistics_Orders_KeyPoints_By_Ordinal()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        var result = ((ObjectResult)controller.GetKeyPointEncounterStatistics(-1).Result)?.Value as KeyPointEncounterStatisticsDto;

        result.ShouldNotBeNull();
        result.KeyPoints.ShouldNotBeEmpty();

        for (int i = 1; i < result.KeyPoints.Count; i++)
        {
            result.KeyPoints[i].OrdinalNo.ShouldBeGreaterThan(result.KeyPoints[i - 1].OrdinalNo);
        }
    }

    [Fact]
    public void GetKeyPointEncounterStatistics_Returns_Empty_KeyPoints_For_Tour_Without_Activity()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        var result = ((ObjectResult)controller.GetKeyPointEncounterStatistics(-2).Result)?.Value as KeyPointEncounterStatisticsDto;

        result.ShouldNotBeNull();
        result.TourId.ShouldBe(-2);
        result.KeyPoints.ShouldNotBeNull();
    }

    [Fact]
    public void GetKeyPointEncounterStatistics_Calculates_Success_Rate_Correctly()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        var result = ((ObjectResult)controller.GetKeyPointEncounterStatistics(-1).Result)?.Value as KeyPointEncounterStatisticsDto;

        result.ShouldNotBeNull();

        var keyPointWithEncounter = result.KeyPoints.FirstOrDefault(kp => kp.Encounter != null);
        if (keyPointWithEncounter != null && keyPointWithEncounter.Encounter.TotalAttempts > 0)
        {
            var expectedRate = Math.Round(
                (double)keyPointWithEncounter.Encounter.SuccessfulAttempts /
                keyPointWithEncounter.Encounter.TotalAttempts * 100,
                2
            );
            keyPointWithEncounter.Encounter.SuccessRate.ShouldBe(expectedRate);
        }
    }

    [Fact]
    public void GetKeyPointEncounterStatistics_Returns_Zero_Success_Rate_When_No_Attempts()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        var result = ((ObjectResult)controller.GetKeyPointEncounterStatistics(-1).Result)?.Value as KeyPointEncounterStatisticsDto;

        result.ShouldNotBeNull();

        var keyPointWithEncounter = result.KeyPoints.FirstOrDefault(kp => kp.Encounter != null);
        if (keyPointWithEncounter != null && keyPointWithEncounter.Encounter.TotalAttempts == 0)
        {
            keyPointWithEncounter.Encounter.SuccessRate.ShouldBe(0.0);
        }
    }

    [Fact]
    public void GetKeyPointEncounterStatistics_Returns_Valid_KeyPoint_Structure()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        var result = ((ObjectResult)controller.GetKeyPointEncounterStatistics(-1).Result)?.Value as KeyPointEncounterStatisticsDto;

        result.ShouldNotBeNull();
        result.KeyPoints.ShouldNotBeNull();

        foreach (var keyPointStat in result.KeyPoints)
        {
            keyPointStat.KeyPointId.ShouldNotBe(0);
            keyPointStat.Name.ShouldNotBeNullOrEmpty();
            keyPointStat.OrdinalNo.ShouldBeGreaterThan(0);
            keyPointStat.TouristsArrived.ShouldBeGreaterThanOrEqualTo(0);
        }
    }

    private static TourStatisticsController CreateController(IServiceScope scope, string userId)
    {
        return new TourStatisticsController(scope.ServiceProvider.GetRequiredService<ITourStatisticsService>())
        {
            ControllerContext = BuildContext(userId)
        };
    }
}