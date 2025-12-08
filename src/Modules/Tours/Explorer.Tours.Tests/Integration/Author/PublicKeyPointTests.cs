using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Author;

[Collection("Sequential")]
public class PublicKeyPointTests : BaseToursIntegrationTest
{
    public PublicKeyPointTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public async Task Author_submits_key_point_for_public_use_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var service = CreatePublicKeyPointService(scope);
        var authorId = -11L;
        var tourId = -11L;
        var ordinalNo = 1; 

        var result = await service.SubmitKeyPointForPublicUseAsync(tourId, ordinalNo, authorId);

        result.ShouldNotBeNull();
        result.Id.ShouldBeGreaterThan(0);
        result.AuthorId.ShouldBe(Math.Abs(authorId)); 
        result.SourceTourId.ShouldBe(tourId);
        result.SourceOrdinalNo.ShouldBe(ordinalNo);
    }

    [Fact]
    public async Task Author_cannot_submit_another_authors_key_point()
    {
        using var scope = Factory.Services.CreateScope();
        var service = CreatePublicKeyPointService(scope);
        var wrongAuthorId = -99L; 
        var tourId = -11L; 
        var ordinalNo = 1;

        await Should.ThrowAsync<UnauthorizedAccessException>(async () =>
            await service.SubmitKeyPointForPublicUseAsync(tourId, ordinalNo, wrongAuthorId));
    }

    [Fact]
    public async Task Author_cannot_submit_same_key_point_twice_when_pending()
    {
        using var scope = Factory.Services.CreateScope();
        var service = CreatePublicKeyPointService(scope);
        var authorId = -11L;
        var tourId = -11L;
        var ordinalNo = 4; 

        await service.SubmitKeyPointForPublicUseAsync(tourId, ordinalNo, authorId);

        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await service.SubmitKeyPointForPublicUseAsync(tourId, ordinalNo, authorId));
    }

    [Fact]
    public async Task Author_can_view_their_submitted_key_points_with_status()
    {
        using var scope = Factory.Services.CreateScope();
        var service = CreatePublicKeyPointService(scope);
        var authorId = -11L;
        var tourId = -11L;
        var ordinalNo = 5; 

        await service.SubmitKeyPointForPublicUseAsync(tourId, ordinalNo, authorId);

        var authorKeyPoints = await service.GetAuthorPublicKeyPointsAsync(authorId);

        var keyPointsList = authorKeyPoints.ToList();
        keyPointsList.ShouldNotBeEmpty();
        keyPointsList.ShouldContain(kp => kp.SourceTourId == tourId && kp.SourceOrdinalNo == ordinalNo);
    }

    [Fact]
    public async Task Throws_exception_when_tour_not_found()
    {
        using var scope = Factory.Services.CreateScope();
        var service = CreatePublicKeyPointService(scope);
        var authorId = -11L;
        var nonExistentTourId = -99999L; 
        var ordinalNo = 1;

        await Should.ThrowAsync<KeyNotFoundException>(async () =>
            await service.SubmitKeyPointForPublicUseAsync(nonExistentTourId, ordinalNo, authorId));
    }

    [Fact]
    public async Task Throws_exception_when_key_point_not_found()
    {
        using var scope = Factory.Services.CreateScope();
        var service = CreatePublicKeyPointService(scope);
        var authorId = -11L;
        var tourId = -11L; 
        var nonExistentOrdinalNo = 99999; 

        await Should.ThrowAsync<KeyNotFoundException>(async () =>
            await service.SubmitKeyPointForPublicUseAsync(tourId, nonExistentOrdinalNo, authorId));
    }

    private static IPublicKeyPointService CreatePublicKeyPointService(IServiceScope scope)
    {
        return scope.ServiceProvider.GetRequiredService<IPublicKeyPointService>();
    }
}