using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration;

[Collection("Sequential")]
public class ClubBadgesCommandTests : BaseStakeholdersIntegrationTest
{
    public ClubBadgesCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Get_returns_badges_from_database()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var badgesController = CreateBadgesController(scope);
        var clubsController = CreateClubsController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var userId = -21; // "ulogovani" turista
        SetUser(badgesController, userId);
        SetUser(clubsController, userId);

        // 1) Kreiramo klub (OwnerId će kontroler postaviti na userId)
        var createdClubResult = clubsController.Create(new ClubDto
        {
            Name = "Klub - badges test",
            Description = "Opis",
            ImageUrls = new List<string> { "img.jpg" }
        });

        var createdClub = (createdClubResult.Result as OkObjectResult)?.Value as ClubDto;
        createdClub.ShouldNotBeNull();
        createdClub.OwnerId.ShouldBe(userId);

        var clubId = createdClub.Id;

        // 2) Ručno upisujemo bedževe u bazu (da test bude determinističan)
        var b1 = new ClubBadge(clubId, 500);
        var b2 = new ClubBadge(clubId, 1000);

        dbContext.Set<ClubBadge>().AddRange(b1, b2);
        dbContext.SaveChanges();

        // Act
        var actionResult = badgesController.Get(clubId);
        var ok = actionResult.Result as OkObjectResult;
        ok.ShouldNotBeNull();

        var milestones = ok.Value as List<int>;
        milestones.ShouldNotBeNull();

        // Assert
        milestones.ShouldContain(500);
        milestones.ShouldContain(1000);
    }

    [Fact]
    public void Get_returns_empty_when_no_badges_exist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var badgesController = CreateBadgesController(scope);
        var clubsController = CreateClubsController(scope);

        var userId = -21;
        SetUser(badgesController, userId);
        SetUser(clubsController, userId);

        // Kreiramo klub bez bedževa
        var createdClubResult = clubsController.Create(new ClubDto
        {
            Name = "Klub - empty badges",
            Description = "Opis",
            ImageUrls = new List<string> { "img.jpg" }
        });

        var createdClub = (createdClubResult.Result as OkObjectResult)?.Value as ClubDto;
        createdClub.ShouldNotBeNull();

        // Act
        var result = badgesController.Get(createdClub.Id);
        var ok = result.Result as OkObjectResult;

        // Assert
        ok.ShouldNotBeNull();
        var milestones = ok.Value as List<int>;
        milestones.ShouldNotBeNull();
        milestones.Count.ShouldBe(0);
    }

    [Fact]
    public void Get_fails_when_user_id_claim_missing()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var badgesController = CreateBadgesController(scope);

        // nema claim "id" niti NameIdentifier
        badgesController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Role, "tourist")
                }, "TestAuth"))
            }
        };

        // Act & Assert
        Should.Throw<System.Exception>(() => badgesController.Get(1))
            .Message.ShouldBe("User id claim not found in token.");
    }

    private static void SetUser(ControllerBase controller, long userId)
    {
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(
                    new ClaimsIdentity(new[]
                    {
                        new Claim("id", userId.ToString()),
                        new Claim(ClaimTypes.Role, "tourist")
                    }, "TestAuth"))
            }
        };
    }

    private static ClubBadgesController CreateBadgesController(IServiceScope scope)
    {
        return new ClubBadgesController(
            scope.ServiceProvider.GetRequiredService<IClubBadgeService>()
        )
        {
            ControllerContext = BuildContext("-21")
        };
    }

    private static ClubsController CreateClubsController(IServiceScope scope)
    {
        return new ClubsController(
            scope.ServiceProvider.GetRequiredService<IClubService>()
        )
        {
            ControllerContext = BuildContext("-21")
        };
    }
}
