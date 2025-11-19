using System.Collections.Generic;
using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration
{
    [Collection("Sequential")]
    public class TourPreferencesQueryTests : BaseStakeholdersIntegrationTest
    {
        public TourPreferencesQueryTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Retrieves_by_tourist_id()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var createDto = new TourPreferencesDto
            {
                TouristId = -21,
                PreferredDifficulty = 0,
                WalkingScore = 3,
                BicycleScore = 2,
                CarScore = 1,
                BoatScore = 0,
                Tags = new List<string> { "nature", "history" }
            };

            var createResult = controller.Create(createDto);
            var createOk = createResult.Result as OkObjectResult;
            var created = createOk?.Value as TourPreferencesDto;
            created.ShouldNotBeNull();

            var actionResult = controller.Get(-21);             
            var okResult = actionResult.Result as OkObjectResult;
            var result = okResult?.Value as TourPreferencesDto;

            result.ShouldNotBeNull();
            result!.TouristId.ShouldBe(-21);
            result.WalkingScore.ShouldBe(createDto.WalkingScore);
            result.PreferredDifficulty.ShouldBe(createDto.PreferredDifficulty);
        }

        private static TourPreferencesController CreateController(IServiceScope scope)
        {
            return new TourPreferencesController(scope.ServiceProvider.GetRequiredService<ITourPreferencesService>())
            {
                ControllerContext = BuildContext("-21")
            };
        }
    }
}
