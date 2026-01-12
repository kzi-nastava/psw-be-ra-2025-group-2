using Explorer.API.Controllers.Tourist;
using Explorer.Payments.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Public.Execution;
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
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var draftTourId = -9999;
        var archivedTourId = -9998;

        var actionResult = controller.GetPublished(page: 1, pageSize: 1000);

        var ok = actionResult.Result as OkObjectResult;
        ok.ShouldNotBeNull();

        var pageDto = ok.Value as PagedResultDto<PublishedTourPreviewDto>;
        pageDto.ShouldNotBeNull();

        var result = pageDto.Results;
        result.ShouldNotBeNull();

        result.Any(t => t.Id == draftTourId).ShouldBeFalse();
        result.Any(t => t.Id == archivedTourId).ShouldBeFalse();
    }

    private static TourController CreateController(IServiceScope scope)
    {
        return new TourController(
            scope.ServiceProvider.GetRequiredService<ITourService>(),
            scope.ServiceProvider.GetRequiredService<IPaymentRecordService>(),
            scope.ServiceProvider.GetRequiredService<ITourExecutionService>());
    }
}
