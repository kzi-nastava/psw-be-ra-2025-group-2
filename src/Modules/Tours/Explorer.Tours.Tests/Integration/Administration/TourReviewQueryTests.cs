using Explorer.API.Controllers.Tourist;
using Explorer.Payments.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Public.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourReviewQueryTests : BaseToursIntegrationTest
{
    public TourReviewQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_all_for_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var actionResult = controller.GetPublished(page: 1, pageSize: 1000);

        var ok = actionResult.Result as OkObjectResult;
        ok.ShouldNotBeNull();

        var pageDto = ok.Value as PagedResultDto<PublishedTourPreviewDto>;
        pageDto.ShouldNotBeNull();

        var tours = pageDto.Results;
        tours.ShouldNotBeNull();
        tours.ShouldNotBeEmpty();

        var targetTour = tours.FirstOrDefault(t => t.Id == -1);
        targetTour.ShouldNotBeNull();

        targetTour.Reviews.ShouldNotBeNull();
        targetTour.Reviews.Count.ShouldBeGreaterThanOrEqualTo(2);

        targetTour.AverageRating.ShouldBeGreaterThan(0);
    }

    private static TourController CreateController(IServiceScope scope)
    {
        return new TourController(
            scope.ServiceProvider.GetRequiredService<ITourService>(),
            scope.ServiceProvider.GetRequiredService<IPaymentRecordService>(),
            scope.ServiceProvider.GetRequiredService<ITourExecutionService>(),
            scope.ServiceProvider.GetRequiredService<ITourPurchaseTokenRepository>()
        )
        {
            ControllerContext = BuildContext("-21")
        };
    }
}
