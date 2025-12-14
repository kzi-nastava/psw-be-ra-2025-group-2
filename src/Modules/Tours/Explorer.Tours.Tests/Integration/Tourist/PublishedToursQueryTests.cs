using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class PublishedToursOnlyPublishedTests : BaseToursIntegrationTest
{
    public PublishedToursOnlyPublishedTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Does_not_return_draft_or_archived_tours_AC1()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // TODO: zameni ovde sa realnim ID-jevima iz vašeg d-tours.sql seed-a
        // npr: draftTourId = -2; archivedTourId = -3;
        var draftTourId = -9999;
        var archivedTourId = -9998;

        // Act
        var response = controller.GetPublished();
        var result = (response.Result as OkObjectResult)?.Value as List<PublishedTourPreviewDto>;

        // Assert
        result.ShouldNotBeNull();

        result.Any(t => t.Id == draftTourId).ShouldBeFalse();
        result.Any(t => t.Id == archivedTourId).ShouldBeFalse();
    }

    private static TourController CreateController(IServiceScope scope)
    {
        return new TourController(
            scope.ServiceProvider.GetRequiredService<ITourService>()
        );
    }
}
