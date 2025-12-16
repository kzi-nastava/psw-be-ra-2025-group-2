using Explorer.API.Controllers.Author;
using Explorer.API.Controllers.Administrator;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.Tours.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Tests.Integration;

[Collection("Sequential")]
public class PublicKeyPointCommandTests : BaseToursIntegrationTest
{
    private const long TestAuthorId = -11;
    private const long TestAdminId = 1;
    private const long TestTourId = -1;
    private const int TestOrdinalNo = 1;
    private const long TestKeyPointId = -100;

    public PublicKeyPointCommandTests(ToursTestFactory factory) : base(factory)
    {
        EnsureTestDataExists();
        CleanupTestData();
    }

    [Fact]
    public async Task Submits_public_keypoint_request_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope);

        var dto = CreateSubmitRequestDto();
        var actionResult = await controller.Submit(dto);

        var request = ExtractResultValue<PublicKeyPointRequestDto>(actionResult);

        request.Status.ShouldBe("Pending");
        request.AuthorId.ShouldBe(TestAuthorId);
    }

    [Fact]
    public async Task Prevents_duplicate_pending_requests()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope);

        var dto = CreateSubmitRequestDto();
        await controller.Submit(dto);
        var actionResult = await controller.Submit(dto);

        actionResult.Result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Gets_author_requests_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope);

        await SubmitTestRequest(controller);
        var actionResult = await controller.GetMyRequests();

        var requests = ExtractResultValue<IEnumerable<PublicKeyPointRequestDto>>(actionResult);

        requests.ShouldNotBeEmpty();
        requests.All(r => r.AuthorId == TestAuthorId).ShouldBeTrue();
    }

    [Fact]
    public async Task Admin_gets_pending_requests_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var adminController = CreateAdminController(scope);
        var authorController = CreateAuthorController(scope);

        await SubmitTestRequest(authorController);
        var actionResult = await adminController.GetPending();

        var requests = ExtractResultValue<IEnumerable<PublicKeyPointRequestDto>>(actionResult);

        requests.ShouldNotBeEmpty();
        requests.All(r => r.Status == "Pending").ShouldBeTrue();
    }

    [Fact]
    public async Task Admin_approves_request_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var adminController = CreateAdminController(scope);
        var authorController = CreateAuthorController(scope);

        var createdRequest = await SubmitTestRequest(authorController);
        var actionResult = await adminController.Approve(createdRequest.Id);

        var approved = ExtractResultValue<PublicKeyPointRequestDto>(actionResult);

        approved.Status.ShouldBe("Approved");
        approved.ProcessedByAdminId.ShouldBe(TestAdminId);
        approved.ProcessedAt.ShouldNotBeNull();

        await VerifyPublicKeyPointStatus(approved.PublicKeyPointId, PublicKeyPointStatus.Approved);
    }

    [Fact]
    public async Task Admin_rejects_request_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var adminController = CreateAdminController(scope);
        var authorController = CreateAuthorController(scope);

        var createdRequest = await SubmitTestRequest(authorController);
        var rejectDto = CreateRejectDto("KeyPoint does not meet quality standards");
        var actionResult = await adminController.Reject(createdRequest.Id, rejectDto);

        var rejected = ExtractResultValue<PublicKeyPointRequestDto>(actionResult);

        rejected.Status.ShouldBe("Rejected");
        rejected.ProcessedByAdminId.ShouldBe(TestAdminId);
        rejected.RejectionReason.ShouldBe("KeyPoint does not meet quality standards");
        rejected.ProcessedAt.ShouldNotBeNull();
    }

    [Fact]
    public async Task Fails_to_submit_request_for_nonexistent_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope);

        var dto = CreateSubmitRequestDto(tourId: 999999);
        var actionResult = await controller.Submit(dto);

        actionResult.Result.ShouldBeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Fails_to_submit_request_for_nonexistent_keypoint()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope);

        var dto = CreateSubmitRequestDto(ordinalNo: 999);
        var actionResult = await controller.Submit(dto);

        actionResult.Result.ShouldBeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Fails_to_approve_already_approved_request()
    {
        using var scope = Factory.Services.CreateScope();
        var adminController = CreateAdminController(scope);
        var authorController = CreateAuthorController(scope);

        var createdRequest = await SubmitTestRequest(authorController);
        await adminController.Approve(createdRequest.Id);
        var actionResult = await adminController.Approve(createdRequest.Id);

        actionResult.Result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Fails_to_approve_non_existent_request()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAdminController(scope);

        var actionResult = await controller.Approve(999999);

        actionResult.Result.ShouldBeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Fails_to_reject_non_existent_request()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAdminController(scope);

        var rejectDto = CreateRejectDto("Test");
        var actionResult = await controller.Reject(999999, rejectDto);

        actionResult.Result.ShouldBeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Creates_notification_when_request_is_approved()
    {
        using var scope = Factory.Services.CreateScope();
        var adminController = CreateAdminController(scope);
        var authorController = CreateAuthorController(scope);

        var createdRequest = await SubmitTestRequest(authorController);
        await adminController.Approve(createdRequest.Id);

        await VerifyNotificationExists(
            NotificationType.PublicKeyPointApproved,
            "KeyPoint Approved");
    }

    [Fact]
    public async Task Creates_notification_when_request_is_rejected()
    {
        using var scope = Factory.Services.CreateScope();
        var adminController = CreateAdminController(scope);
        var authorController = CreateAuthorController(scope);

        var createdRequest = await SubmitTestRequest(authorController);
        var rejectDto = CreateRejectDto("Quality standards not met");
        await adminController.Reject(createdRequest.Id, rejectDto);

        await VerifyNotificationExists(
            NotificationType.PublicKeyPointRejected,
            "KeyPoint Request Rejected",
            "Quality standards not met");
    }


    private void EnsureTestDataExists()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        if (TourExists(db))
            return;

        CreateTestTour(db);
        db.SaveChanges();
    }

    private static bool TourExists(ToursContext db)
    {
        return db.Tours
            .Include(t => t.KeyPoints)
            .Any(t => t.Id == TestTourId);
    }

    private static void CreateTestTour(ToursContext db)
    {
        var tour = new Tour("Test Tour", "This is a test tour", 1, TestAuthorId);
        SetEntityId(tour, TestTourId);
        tour.SetStatus(TourStatus.Published);

        var keyPoint = new KeyPoint(
            TestOrdinalNo,
            "Test KeyPoint",
            "Test Description",
            "Secret",
            "test.jpg",
            45.0,
            19.0,
            TestAuthorId
        );
        SetEntityId(keyPoint, TestKeyPointId);

        var keyPointsField = typeof(Tour).GetField("_keyPoints",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var keyPointsList = new List<KeyPoint> { keyPoint };
        keyPointsField?.SetValue(tour, keyPointsList);

        db.Tours.Add(tour);
    }

    private void CleanupTestData()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        RemoveTestRequests(db);
        RemoveTestPublicKeyPoints(db);
        RemoveTestNotifications(db);

        db.SaveChanges();
    }

    private static void RemoveTestRequests(ToursContext db)
    {
        var requests = db.PublicKeyPointRequests.ToList();
        db.PublicKeyPointRequests.RemoveRange(requests);
    }

    private static void RemoveTestPublicKeyPoints(ToursContext db)
    {
        var publicKeyPoints = db.PublicKeyPoints.ToList();
        db.PublicKeyPoints.RemoveRange(publicKeyPoints);
    }

    private static void RemoveTestNotifications(ToursContext db)
    {
        var notifications = db.Notifications
            .Where(n => n.UserId == TestAuthorId)
            .ToList();
        db.Notifications.RemoveRange(notifications);
    }


    private static async Task<PublicKeyPointRequestDto> SubmitTestRequest(
        PublicKeyPointController controller)
    {
        var dto = CreateSubmitRequestDto();
        var actionResult = await controller.Submit(dto);
        return ExtractResultValue<PublicKeyPointRequestDto>(actionResult);
    }

    private async Task VerifyPublicKeyPointStatus(
        long publicKeyPointId,
        PublicKeyPointStatus expectedStatus)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var publicKeyPoint = await db.PublicKeyPoints
            .FirstOrDefaultAsync(pkp => pkp.Id == publicKeyPointId);

        publicKeyPoint.ShouldNotBeNull();
        publicKeyPoint!.Status.ShouldBe(expectedStatus);
    }

    private async Task VerifyNotificationExists(
        NotificationType type,
        string expectedTitle,
        string? expectedMessageContent = null)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var notification = await db.Notifications
            .FirstOrDefaultAsync(n => n.UserId == TestAuthorId && n.Type == type);

        notification.ShouldNotBeNull();
        notification!.Title.ShouldBe(expectedTitle);
        notification.IsRead.ShouldBeFalse();

        if (expectedMessageContent != null)
        {
            notification.Message.ShouldContain(expectedMessageContent);
        }
    }


    private static SubmitKeyPointRequestDto CreateSubmitRequestDto(
        long? tourId = null,
        int? ordinalNo = null)
    {
        return new SubmitKeyPointRequestDto
        {
            TourId = tourId ?? TestTourId,
            OrdinalNo = ordinalNo ?? TestOrdinalNo
        };
    }

    private static RejectRequestDto CreateRejectDto(string reason)
    {
        return new RejectRequestDto { Reason = reason };
    }


    private static T ExtractResultValue<T>(ActionResult<T> actionResult) where T : class
    {
        var okResult = actionResult.Result as OkObjectResult;
        okResult.ShouldNotBeNull();

        var value = okResult!.Value as T;
        value.ShouldNotBeNull();

        return value!;
    }


    private static PublicKeyPointController CreateAuthorController(IServiceScope scope)
    {
        return new PublicKeyPointController(
            scope.ServiceProvider.GetRequiredService<IPublicKeyPointService>())
        {
            ControllerContext = BuildContext(TestAuthorId.ToString())
        };
    }

    private static PublicKeyPointRequestController CreateAdminController(IServiceScope scope)
    {
        return new PublicKeyPointRequestController(
            scope.ServiceProvider.GetRequiredService<IPublicKeyPointService>())
        {
            ControllerContext = BuildContext(TestAdminId.ToString())
        };
    }


    private static void SetEntityId<T>(T entity, long id) where T : class
    {
        typeof(T).BaseType?.GetProperty("Id")?.SetValue(entity, id);
    }


}