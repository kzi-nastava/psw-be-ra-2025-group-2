using Explorer.Tours.API.Public;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Administrator;

[Collection("Sequential")]
public class PublicKeyPointAdminTests : BaseToursIntegrationTest
{
    public PublicKeyPointAdminTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public async Task Admin_can_view_pending_requests()
    {
        using var scope = Factory.Services.CreateScope();
        var service = CreatePublicKeyPointService(scope);

        await service.SubmitKeyPointForPublicUseAsync(7L, 4, 11L);

        var pendingRequests = await service.GetPendingRequestsAsync();

        var requestsList = pendingRequests.ToList();
        requestsList.ShouldNotBeEmpty();
        requestsList.ShouldAllBe(r => r.RequestStatus == "Pending");
    }

    [Fact]
    public async Task Admin_approves_key_point_request_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var service = CreatePublicKeyPointService(scope);
        var adminId = 1L;

        var submitted = await service.SubmitKeyPointForPublicUseAsync(7L, 5, 11L);
        var publicKeyPointId = submitted.Id;

        var result = await service.ApproveAsync(publicKeyPointId, adminId);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(publicKeyPointId);

        var approved = await service.GetApprovedPublicKeyPointsAsync();
        approved.ShouldContain(kp => kp.Id == publicKeyPointId);
    }

    [Fact]
    public async Task Admin_rejects_key_point_request_with_reason()
    {
        using var scope = Factory.Services.CreateScope();
        var service = CreatePublicKeyPointService(scope);
        var adminId = 1L;
        var rejectionReason = "Insufficient details provided";

        var submitted = await service.SubmitKeyPointForPublicUseAsync(7L, 6, 11L);
        var publicKeyPointId = submitted.Id;

        var result = await service.RejectAsync(publicKeyPointId, adminId, rejectionReason);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(publicKeyPointId);

        var pending = await service.GetPendingRequestsAsync();
        pending.ShouldNotContain(kp => kp.PublicKeyPointId == publicKeyPointId);
    }

    [Fact]
    public async Task Admin_cannot_approve_non_existent_request()
    {
        using var scope = Factory.Services.CreateScope();
        var service = CreatePublicKeyPointService(scope);
        var adminId = 1L;
        var nonExistentId = 99999L;

        await Should.ThrowAsync<KeyNotFoundException>(async () =>
            await service.ApproveAsync(nonExistentId, adminId));
    }

    [Fact]
    public async Task Author_can_add_approved_public_key_point_to_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var service = CreatePublicKeyPointService(scope);
        var authorId = 11L;
        var adminId = 1L;

        var submitted = await service.SubmitKeyPointForPublicUseAsync(7L, 7, authorId);
        await service.ApproveAsync(submitted.Id, adminId);

        await Should.NotThrowAsync(async () =>
            await service.AddPublicKeyPointToTourAsync(submitted.Id, 7L, 20, authorId));
    }

    [Fact]
    public async Task Author_cannot_add_pending_public_key_point_to_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var service = CreatePublicKeyPointService(scope);
        var authorId = 11L;

        var submitted = await service.SubmitKeyPointForPublicUseAsync(7L, 8, authorId);

        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await service.AddPublicKeyPointToTourAsync(submitted.Id, 7L, 21, authorId));
    }

    private static IPublicKeyPointService CreatePublicKeyPointService(IServiceScope scope)
    {
        return scope.ServiceProvider.GetRequiredService<IPublicKeyPointService>();
    }
}