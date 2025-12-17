using Explorer.API.Controllers.Administrator;
using Explorer.API.Controllers.Author;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Integration;

[Collection("Sequential")]
[Trait("Category", "MyTests")]
public class PublicKeyPointCommandTests : BaseToursIntegrationTest, IDisposable
{
    private const long TestAuthorId = -11;
    private const long TestAdminId = 1;
    private const long TestTourId = -1;
    private const int TestOrdinalNo = 1;
    private const long TestKeyPointId = -100;

    private readonly IServiceScope _scope;

    public PublicKeyPointCommandTests(ToursTestFactory factory) : base(factory)
    {
        _scope = Factory.Services.CreateScope();
        SetupDatabase();
    }

    public void Dispose()
    {
        CleanupDatabase();
        _scope.Dispose();
    }

    // --- Svi tvoji testovi ostaju nepromenjeni ---
    // ...
    // --- (Ovde ide tvojih 15 testova, od Submits_public_keypoint_request_successfully do Creates_notification_when_request_is_rejected) ---
    // ...
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

    // --- Metode za upravljanje bazom podataka ---

    private void SetupDatabase()
    {
        CleanupDatabase();
        var dbContext = _scope.ServiceProvider.GetRequiredService<ToursContext>();
        EnsureTestDataExists(dbContext);
    }

    private void CleanupDatabase()
    {
        var dbContext = _scope.ServiceProvider.GetRequiredService<ToursContext>();

        // BRIŠENJE U ISPRAVNOM REDOSLEDU DA SE IZBEGNU FOREIGN KEY GREŠKE
        // Prvo se brišu podaci iz tabela koje imaju spoljne ključeve ka drugima
        dbContext.PublicKeyPointRequests.RemoveRange(dbContext.PublicKeyPointRequests);
        dbContext.PublicKeyPoints.RemoveRange(dbContext.PublicKeyPoints);
        dbContext.Notifications.RemoveRange(dbContext.Notifications);
        dbContext.TourReviews.RemoveRange(dbContext.TourReviews);
        dbContext.TouristEquipment.RemoveRange(dbContext.TouristEquipment);

        // Zatim se brišu podaci iz glavnih tabela
        // KeyPoints se brišu kaskadno sa Tour, tako da ne moramo eksplicitno
        dbContext.Tours.RemoveRange(dbContext.Tours);
        dbContext.Equipment.RemoveRange(dbContext.Equipment);
        dbContext.Monument.RemoveRange(dbContext.Monument);
        dbContext.TouristObject.RemoveRange(dbContext.TouristObject);

        dbContext.SaveChanges();
    }

    private void EnsureTestDataExists(ToursContext db)
    {
        if (db.Tours.Any(t => t.Id == TestTourId)) return;
        CreateTestTour(db);
        db.SaveChanges();
    }

    // --- Pomoćne metode (Helpers) ---

    private static void CreateTestTour(ToursContext db)
    {
        var tour = new Tour("Test Tour", "This is a test tour", 1, TestAuthorId);
        SetEntityId(tour, TestTourId);
        tour.SetStatus(TourStatus.Published);

        var keyPoint = new KeyPoint(
            TestOrdinalNo, "Test KeyPoint", "Test Description", "Secret",
            "test.jpg", 45.0, 19.0, TestAuthorId, false
        );
        SetEntityId(keyPoint, TestKeyPointId);

        var keyPointsField = typeof(Tour).GetField("_keyPoints",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        keyPointsField?.SetValue(tour, new List<KeyPoint> { keyPoint });

        db.Tours.Add(tour);
    }

    // ... Ostatak tvojih pomoćnih metoda (Helpers) ostaje nepromenjen ...
    private static async Task<PublicKeyPointRequestDto> SubmitTestRequest(PublicKeyPointController controller)
    {
        var dto = CreateSubmitRequestDto();
        var actionResult = await controller.Submit(dto);
        return ExtractResultValue<PublicKeyPointRequestDto>(actionResult);
    }

    private async Task VerifyPublicKeyPointStatus(long publicKeyPointId, PublicKeyPointStatus expectedStatus)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var publicKeyPoint = await db.PublicKeyPoints.FirstOrDefaultAsync(pkp => pkp.Id == publicKeyPointId);

        publicKeyPoint.ShouldNotBeNull();
        publicKeyPoint!.Status.ShouldBe(expectedStatus);
    }

    private async Task VerifyNotificationExists(NotificationType type, string expectedTitle, string? expectedMessageContent = null)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var notification = await db.Notifications.FirstOrDefaultAsync(n => n.UserId == TestAuthorId && n.Type == type);

        notification.ShouldNotBeNull();
        notification!.Title.ShouldBe(expectedTitle);
        notification.IsRead.ShouldBeFalse();

        if (expectedMessageContent != null)
        {
            notification.Message.ShouldContain(expectedMessageContent);
        }
    }

    private static SubmitKeyPointRequestDto CreateSubmitRequestDto(long? tourId = null, int? ordinalNo = null)
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
        if (actionResult.Result is BadRequestObjectResult badRequest)
        {
            var errorMessage = badRequest.Value?.ToString() ?? "Unknown error";
            throw new Exception($"❌ Request failed with BadRequest: {errorMessage}");
        }

        if (actionResult.Result is NotFoundObjectResult notFound)
        {
            var errorMessage = notFound.Value?.ToString() ?? "Unknown error";
            throw new Exception($"❌ Request failed with NotFound: {errorMessage}");
        }

        var okResult = actionResult.Result as OkObjectResult;

        if (okResult == null)
        {
            var resultType = actionResult.Result?.GetType().Name ?? "null";
            throw new Exception($"❌ Expected OkResult but got: {resultType}");
        }

        var value = okResult.Value as T;
        value.ShouldNotBeNull($"❌ OkResult.Value is not of type {typeof(T).Name}");

        return value!;
    }

    private static PublicKeyPointController CreateAuthorController(IServiceScope scope)
    {
        return new PublicKeyPointController(scope.ServiceProvider.GetRequiredService<IPublicKeyPointService>())
        {
            ControllerContext = BuildContext(TestAuthorId.ToString())
        };
    }

    private static PublicKeyPointRequestController CreateAdminController(IServiceScope scope)
    {
        return new PublicKeyPointRequestController(scope.ServiceProvider.GetRequiredService<IPublicKeyPointService>())
        {
            ControllerContext = BuildContext(TestAdminId.ToString())
        };
    }

    private static void SetEntityId<T>(T entity, long id) where T : class
    {
        var property = typeof(T).BaseType?.GetProperty("Id") ?? typeof(T).GetProperty("Id");
        property?.SetValue(entity, id);
    }
}