using System;
using System.Linq;
using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Meetups;

[Collection("Sequential")]
public class TouristMeetupCommandTests : BaseStakeholdersIntegrationTest
{
    public TouristMeetupCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Tourist_Creates_meetup()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var newEntity = new MeetupDto
        {
            Name = "Tourist Created Meetup",
            Description = "Markdown **tourist** meetup",
            Date = new DateTime(2025, 07, 12, 17, 0, 0),
            Latitude = 44.82,
            Longitude = 20.47
        };

        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as MeetupDto;

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);

        var stored = db.Meetups.FirstOrDefault(x => x.Name == newEntity.Name);
        stored.ShouldNotBeNull();
        stored.Name.ShouldBe(newEntity.Name);
        stored.CreatorId.ShouldBe(-21); // Tourist -21
    }

    [Fact]
    public void Tourist_Update_fails_nonexistent()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var invalid = new MeetupDto { Id = -999, Name = "X" };

        Should.Throw<NotFoundException>(() => controller.Update(-999, invalid));
    }

    [Fact]
    public void Tourist_Deletes_meetup()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var result = (OkResult)controller.Delete(-103);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        var stored = db.Meetups.FirstOrDefault(m => m.Id == -103);
        stored.ShouldBeNull();
    }

    private static TouristMeetupController CreateController(IServiceScope scope)
    {
        return new TouristMeetupController(scope.ServiceProvider.GetRequiredService<IMeetupService>())
        {
            ControllerContext = BuildContext("-21")
        };
    }
}