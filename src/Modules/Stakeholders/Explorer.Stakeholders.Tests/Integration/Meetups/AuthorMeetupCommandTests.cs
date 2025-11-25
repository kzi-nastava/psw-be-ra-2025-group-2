using System;
using System.Linq;
using Explorer.API.Controllers.Author;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Meetups;

[Collection("Sequential")]
public class AuthorMeetupCommandTests : BaseStakeholdersIntegrationTest
{
    public AuthorMeetupCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    /*[Fact]
    public void Author_Creates_meetup()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var newEntity = new MeetupDto
        {
            Name = "Author Created Meetup",
            Description = "Markdown **author** meetup",
            Date = new DateTime(2025, 06, 10, 18, 0, 0),
            Latitude = 44.81,
            Longitude = 20.46
        };

        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as MeetupDto;

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);

        var stored = db.Meetups.FirstOrDefault(x => x.Name == newEntity.Name);
        stored.ShouldNotBeNull();
        stored.Name.ShouldBe(newEntity.Name);
        stored.CreatorId.ShouldBe(-11); // Author -11
    }*/

    [Fact]
    public void Author_Create_fails_invalid_data()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var invalid = new MeetupDto { Description = "No name" };

        Should.Throw<ArgumentException>(() => controller.Create(invalid));
    }

    /*[Fact]
    public void Author_Updates_meetup()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var updated = new MeetupDto
        {
            Id = -101, // from test data
            Name = "Author Updated Meetup",
            Description = "Updated by author",
            Date = new DateTime(2025, 2, 2),
            Latitude = 44.78,
            Longitude = 20.45
        };

        var result = ((ObjectResult)controller.Update(-101, updated).Result)?.Value as MeetupDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(-101);
        result.Name.ShouldBe(updated.Name);

        var stored = db.Meetups.FirstOrDefault(m => m.Id == -101);
        stored.ShouldNotBeNull();
        stored.Name.ShouldBe(updated.Name);
    }*/

    [Fact]
    public void Author_Deletes_meetup()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var result = (OkResult)controller.Delete(-101);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        var stored = db.Meetups.FirstOrDefault(m => m.Id == -101);
        stored.ShouldBeNull();
    }

    /*[Fact]
    public void Author_Delete_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        Should.Throw<NotFoundException>(() => controller.Delete(-999));
    }*/

    private static AuthorMeetupController CreateController(IServiceScope scope)
    {
        return new AuthorMeetupController(scope.ServiceProvider.GetRequiredService<IMeetupService>())
        {
            ControllerContext = BuildContext("-11")
        };
    }
}