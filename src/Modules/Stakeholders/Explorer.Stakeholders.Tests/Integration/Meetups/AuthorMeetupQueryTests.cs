using System.Collections.Generic;
using System.Linq;
using Explorer.API.Controllers.Author;
using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Meetups;

[Collection("Sequential")]
public class AuthorMeetupQueryTests : BaseStakeholdersIntegrationTest
{
    public AuthorMeetupQueryTests(StakeholdersTestFactory factory) : base(factory) { }

    /*[Fact]
    public void Author_Retrieves_all_meetups()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetAll().Result)?.Value as IEnumerable<MeetupDto>;

        result.ShouldNotBeNull();
        result.Count().ShouldBeGreaterThanOrEqualTo(3); // test data has at least 3
    }*/

    [Fact]
    public void Author_Retrieves_own_meetups()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetMine().Result)?.Value as IEnumerable<MeetupDto>;

        result.ShouldNotBeNull();
        result.All(m => m.CreatorId == -11).ShouldBeTrue();
    }

    private static TouristMeetupController CreateController(IServiceScope scope)
    {
        return new TouristMeetupController(scope.ServiceProvider.GetRequiredService<IMeetupService>())
        {
            ControllerContext = BuildContext("-11")
        };
    }
}