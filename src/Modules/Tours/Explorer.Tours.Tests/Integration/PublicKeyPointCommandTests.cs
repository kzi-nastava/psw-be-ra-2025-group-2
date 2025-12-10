using Explorer.API.Controllers.Author;
using Explorer.API.Controllers.Administrator;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Tests.Integration;

[Collection("Sequential")]
public class PublicKeyPointCommandTests : BaseToursIntegrationTest
{
    private const long TestAuthorId = -11;
    private const long TestAdminId = -1;
    private const long TestTourId = -2;
    private const int TestOrdinalNo = 1;

    public PublicKeyPointCommandTests(ToursTestFactory factory) : base(factory)
    {
        SeedTestData();
    }

    private void SeedTestData()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Proveri da li KeyPoints već postoje - koristi ExecuteSqlRaw sa OUT parametrom
        try
        {
            // Jednostavniji pristup - pokušaj INSERT, ignoriši ako već postoje
            dbContext.Database.ExecuteSqlRaw(@"
            INSERT INTO tours.""KeyPoint""
            (""TourId"", ""OrdinalNo"", ""Name"", ""Description"", ""SecretText"", ""ImageUrl"", 
             ""Latitude"", ""Longitude"", ""AuthorId"", ""PublicStatus"")
            SELECT -2, 1, 'Petrovaradin Fortress', 'Historic fortress overlooking Danube', 
                   'Secret passage leads to underground tunnels', 'https://example.com/fortress.jpg', 
                   45.2517, 19.8661, -11, 'Draft'
            WHERE NOT EXISTS (SELECT 1 FROM tours.""KeyPoint"" WHERE ""TourId"" = -2 AND ""OrdinalNo"" = 1);
            
            INSERT INTO tours.""KeyPoint""
            (""TourId"", ""OrdinalNo"", ""Name"", ""Description"", ""SecretText"", ""ImageUrl"", 
             ""Latitude"", ""Longitude"", ""AuthorId"", ""PublicStatus"")
            SELECT -2, 2, 'Dunavska Street', 'Main pedestrian street in city center', 
                   'Best ice cream shop at number 15', 'https://example.com/dunavska.jpg', 
                   45.2551, 19.8451, -11, 'Draft'
            WHERE NOT EXISTS (SELECT 1 FROM tours.""KeyPoint"" WHERE ""TourId"" = -2 AND ""OrdinalNo"" = 2);
        ");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SeedTestData warning: {ex.Message}");
            // Ignoriši grešku ako podaci već postoje
        }
    }

    private void CleanupTestData()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Obriši test podatke
        dbContext.Database.ExecuteSqlRaw(@"
            DELETE FROM tours.""PublicKeyPointRequests"";
            DELETE FROM tours.""PublicKeyPoints"";
        ");
    }



    [Fact]
    public async Task Submits_public_keypoint_request_successfully()
    {
        CleanupTestData();

        using var scope = Factory.Services.CreateScope();
        var authorController = CreateAuthorController(scope);

        var dto = new SubmitKeyPointRequestDto
        {
            TourId = TestTourId,
            OrdinalNo = TestOrdinalNo
        };

        var actionResult = await authorController.Submit(dto);
        var result = actionResult.Result as ObjectResult;
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        var created = result.Value as PublicKeyPointRequestDto;
        created.ShouldNotBeNull();
        created!.Id.ShouldNotBe(0);
        created.PublicKeyPointId.ShouldNotBe(0);
        created.AuthorId.ShouldBe(TestAuthorId);
        created.Status.ShouldBe("Pending");
        created.CreatedAt.ShouldNotBe(default);
    }

    [Fact]
    public async Task Gets_author_requests_successfully()
    {
        CleanupTestData();

        using var scope = Factory.Services.CreateScope();
        var authorController = CreateAuthorController(scope);

        var submitDto = new SubmitKeyPointRequestDto { TourId = TestTourId, OrdinalNo = TestOrdinalNo };
        await authorController.Submit(submitDto);

        var actionResult = await authorController.GetMyRequests();
        var result = actionResult.Result as ObjectResult;
        result.ShouldNotBeNull();

        var requests = result.Value as IEnumerable<PublicKeyPointRequestDto>;
        requests.ShouldNotBeNull();
        requests!.Any(r => r.AuthorId == TestAuthorId).ShouldBeTrue();
    }

    [Fact]
    public async Task Admin_gets_pending_requests_successfully()
    {
        CleanupTestData();

        using var scope = Factory.Services.CreateScope();
        var adminController = CreateAdminController(scope);
        var authorController = CreateAuthorController(scope);

        var submitDto = new SubmitKeyPointRequestDto { TourId = TestTourId, OrdinalNo = TestOrdinalNo };
        await authorController.Submit(submitDto);

        var actionResult = await adminController.GetPending();
        var result = actionResult.Result as ObjectResult;
        result.ShouldNotBeNull();

        var requests = result.Value as IEnumerable<PublicKeyPointRequestDto>;
        requests.ShouldNotBeNull();
        requests!.All(r => r.Status == "Pending").ShouldBeTrue();
    }

    [Fact]
    public async Task Admin_approves_request_successfully()
    {
        CleanupTestData();

        using var scope = Factory.Services.CreateScope();
        var adminController = CreateAdminController(scope);
        var authorController = CreateAuthorController(scope);

        var submitDto = new SubmitKeyPointRequestDto { TourId = TestTourId, OrdinalNo = TestOrdinalNo };
        var submitAction = await authorController.Submit(submitDto);

        var submitResult = submitAction.Result as ObjectResult;
        submitResult.ShouldNotBeNull();
        submitResult.StatusCode.ShouldBe(200);

        var createdRequest = submitResult.Value as PublicKeyPointRequestDto;
        createdRequest.ShouldNotBeNull();

        var actionResult = await adminController.Approve(createdRequest!.Id);
        var result = actionResult.Result as ObjectResult;
        result.ShouldNotBeNull();

        var approved = result.Value as PublicKeyPointRequestDto;
        approved.ShouldNotBeNull();
        approved!.Status.ShouldBe("Approved");
        approved.ProcessedByAdminId.ShouldBe(TestAdminId);
        approved.ProcessedAt.ShouldNotBeNull();
    }

    [Fact]
    public async Task Admin_rejects_request_successfully()
    {
        CleanupTestData();

        using var scope = Factory.Services.CreateScope();
        var adminController = CreateAdminController(scope);
        var authorController = CreateAuthorController(scope);

        var submitDto = new SubmitKeyPointRequestDto { TourId = TestTourId, OrdinalNo = TestOrdinalNo };
        var submitAction = await authorController.Submit(submitDto);

        var submitResult = submitAction.Result as ObjectResult;
        submitResult.ShouldNotBeNull();

        var createdRequest = submitResult.Value as PublicKeyPointRequestDto;
        createdRequest.ShouldNotBeNull();

        var rejectDto = new RejectDto { Reason = "Keypoint does not meet quality standards" };
        var result = await adminController.Reject(createdRequest!.Id, rejectDto);
        var obj = result.Result as ObjectResult;
        obj.ShouldNotBeNull();

        var rejected = obj.Value as PublicKeyPointRequestDto;
        rejected.ShouldNotBeNull();
        rejected!.Status.ShouldBe("Rejected");
        rejected.ProcessedByAdminId.ShouldBe(TestAdminId);
        rejected.RejectionReason.ShouldBe("Keypoint does not meet quality standards");
        rejected.ProcessedAt.ShouldNotBeNull();
    }

    [Fact]
    public async Task Fails_to_submit_request_for_nonexistent_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope);

        var dto = new SubmitKeyPointRequestDto { TourId = 999999, OrdinalNo = 1 };
        var result = await controller.Submit(dto);
        var obj = result.Result as ObjectResult;
        obj.ShouldNotBeNull();
        obj.StatusCode.ShouldBeOneOf(400, 500);
    }

    [Fact]
    public async Task Fails_to_submit_request_for_nonexistent_keypoint()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope);

        var dto = new SubmitKeyPointRequestDto { TourId = TestTourId, OrdinalNo = 999 };
        var result = await controller.Submit(dto);
        var obj = result.Result as ObjectResult;
        obj.ShouldNotBeNull();
        obj.StatusCode.ShouldBeOneOf(400, 500);
    }

    [Fact]
    public async Task Fails_to_approve_non_existent_request()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAdminController(scope);

        var result = await controller.Approve(999999);
        var obj = result.Result as ObjectResult;
        obj.ShouldNotBeNull();
        obj.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task Fails_to_reject_non_existent_request()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAdminController(scope);

        var rejectDto = new RejectDto { Reason = "Test" };
        var result = await controller.Reject(999999, rejectDto);
        var obj = result.Result as ObjectResult;
        obj.ShouldNotBeNull();
        obj.StatusCode.ShouldBe(404);
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
}