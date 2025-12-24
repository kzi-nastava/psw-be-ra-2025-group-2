using Explorer.API.Controllers.Author;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Tours.Tests.Integration.Tour;

[Collection("Sequential")]
public class PublicKeyPointCommandTests : BaseToursIntegrationTest
{
    public PublicKeyPointCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public async Task Submits_keypoint_for_public_approval()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var submitDto = new SubmitKeyPointRequestDto
        {
            TourId = -15,
            OrdinalNo = 1
        };

        // Act
        var actionResult = await controller.Submit(submitDto);

        if (actionResult.Result is BadRequestObjectResult badRequest)
        {
            var errorObj = badRequest.Value;
            Console.WriteLine($"❌ BadRequest Error: {errorObj}");
            var errorType = errorObj?.GetType();
            if (errorType?.GetProperty("error") != null)
            {
                var errorMsg = errorType.GetProperty("error")?.GetValue(errorObj);
                Console.WriteLine($"❌ Error Message: {errorMsg}");
            }
        }

        // Assert - Response
        actionResult.Result.ShouldBeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)actionResult.Result;
        var result = okResult.Value as PublicKeyPointRequestDto;

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Status.ShouldBe("Pending");
        result.AuthorId.ShouldBe(-12);

        // Assert - Database
        var storedRequest = dbContext.PublicKeyPointRequests.FirstOrDefault(r => r.Id == result.Id);
        storedRequest.ShouldNotBeNull();
        storedRequest.Status.ToString().ShouldBe("Pending");
    }

    [Fact]
    public async Task Fails_when_tour_does_not_exist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var submitDto = new SubmitKeyPointRequestDto
        {
            TourId = 99999,
            OrdinalNo = 2
        };

        // Act
        var actionResult = await controller.Submit(submitDto);

        // Assert
        actionResult.Result.ShouldBeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)actionResult.Result;
        notFoundResult.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task Fails_when_keypoint_does_not_exist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var submitDto = new SubmitKeyPointRequestDto
        {
            TourId = -15,
            OrdinalNo = 999
        };

        // Act
        var actionResult = await controller.Submit(submitDto);

        // Assert
        actionResult.Result.ShouldBeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)actionResult.Result;
        notFoundResult.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task Fails_when_keypoint_already_has_pending_request()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope); 

        var submitDto = new SubmitKeyPointRequestDto
        {
            TourId = -15,
            OrdinalNo = 2
        };

        var firstActionResult = await controller.Submit(submitDto);
        firstActionResult.Result.ShouldBeOfType<OkObjectResult>();

        var secondActionResult = await controller.Submit(submitDto);

        // Assert
        secondActionResult.Result.ShouldBeOfType<BadRequestObjectResult>();
        var badRequestResult = (BadRequestObjectResult)secondActionResult.Result;
        badRequestResult.StatusCode.ShouldBe(400);
    }

    [Fact]
    public async Task Gets_author_requests()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var submitDto = new SubmitKeyPointRequestDto
        {
            TourId = -15,
            OrdinalNo = 3
        };
        await controller.Submit(submitDto);

        // Act
        var actionResult = await controller.GetMyRequests();

        // Assert - Response
        actionResult.Result.ShouldBeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)actionResult.Result;
        var result = okResult.Value as IEnumerable<PublicKeyPointRequestDto>;

        result.ShouldNotBeNull();
        result.Count().ShouldBeGreaterThan(0);
        result.All(r => r.AuthorId == -12).ShouldBeTrue();

        // Assert - Database
        var storedRequests = dbContext.PublicKeyPointRequests.Where(r => r.AuthorId == -12).ToList();
        storedRequests.Count.ShouldBe(result.Count());
    }

    private static PublicKeyPointController CreateController(IServiceScope scope)
    {
        return new PublicKeyPointController(
            scope.ServiceProvider.GetRequiredService<IPublicKeyPointService>())
        {
            ControllerContext = BuildContext("-12")
        };
    }
}