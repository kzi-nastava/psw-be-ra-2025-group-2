using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;
using System.Linq;
using System.Collections.Generic;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration
{
    [Collection("Sequential")]
    public class ClubLeaderboardQueryTests : BaseStakeholdersIntegrationTest
    {
        public ClubLeaderboardQueryTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void GetLeaderboard_returns_rows_for_valid_club()
        {
            using var scope = Factory.Services.CreateScope();

            var userId = -21; 
            var clubsController = CreateClubsController(scope, userId);
            var leaderboardController = CreateLeaderboardController(scope, userId);

            var created = clubsController.Create(new ClubDto
            {
                Name = "LB test club",
                Description = "Club for leaderboard test",
                ImageUrls = new List<string> { "img.jpg" }
            });

            var createdDto = (created.Result as OkObjectResult)?.Value as ClubDto;
            createdDto.ShouldNotBeNull();
            createdDto.Id.ShouldBeGreaterThan(0);

            var actionResult = leaderboardController.GetLeaderboard(createdDto.Id);
            var rows = (actionResult.Result as OkObjectResult)?.Value as List<ClubLeaderboardRowDto>;

            rows.ShouldNotBeNull();
            rows.ShouldNotBeNull();

            if (rows.Count > 0)
            {
                rows.All(r => r.Rank > 0).ShouldBeTrue();
                rows.All(r => r.TotalXp >= 0).ShouldBeTrue();

                rows.Select(r => r.Rank).Distinct().Count().ShouldBe(rows.Count);
                rows.Min(r => r.Rank).ShouldBe(1);
            }
        }

        [Fact]
        public void GetClubsLeaderboard_returns_list()
        {
            using var scope = Factory.Services.CreateScope();

            var userId = -21;
            var controller = CreateLeaderboardController(scope, userId);

            var actionResult = controller.GetClubsLeaderboard();
            var rows = (actionResult.Result as OkObjectResult)?.Value as List<ClubLeaderboardClubRowDto>;

            rows.ShouldNotBeNull();

            rows.ShouldNotBeNull();

            if (rows.Count > 0)
            {
                rows.All(r => r.Rank > 0).ShouldBeTrue();
                rows.All(r => r.ClubId > 0).ShouldBeTrue();
                rows.All(r => !string.IsNullOrWhiteSpace(r.ClubName)).ShouldBeTrue();
                rows.All(r => r.MembersCount >= 0).ShouldBeTrue();
                rows.All(r => r.TotalXp >= 0).ShouldBeTrue();

                rows.Select(r => r.Rank).Distinct().Count().ShouldBe(rows.Count);

                rows.Select(r => r.Rank).SequenceEqual(rows.Select(r => r.Rank).OrderBy(x => x)).ShouldBeTrue();
            }
        }

        private static ClubLeaderboardController CreateLeaderboardController(IServiceScope scope, long userId)
        {
            var controller = new ClubLeaderboardController(
                scope.ServiceProvider.GetRequiredService<IClubLeaderboardService>())
            {
                ControllerContext = BuildContext(userId.ToString())
            };

            return controller;
        }

        private static ClubsController CreateClubsController(IServiceScope scope, long userId)
        {
            var controller = new ClubsController(scope.ServiceProvider.GetRequiredService<IClubService>())
            {
                ControllerContext = BuildContext(userId.ToString())
            };

            return controller;
        }

        private static ControllerContext BuildContext(string userId)
        {
            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(new[]
                        {
                            new Claim("id", userId),
                            new Claim(ClaimTypes.Role, "tourist")
                        }, "TestAuth"))
                }
            };
        }
    }
}
