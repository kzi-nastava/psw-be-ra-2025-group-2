using Explorer.API.Controllers.Administrator;
using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using System.Linq;

namespace Explorer.Stakeholders.Tests.Integration
{
    [Collection("Sequential")]
    public class AppRatingTests : BaseStakeholdersIntegrationTest
    {
        private readonly StakeholdersContext _dbContext;

        public AppRatingTests(StakeholdersTestFactory factory) : base(factory)
        {
            var scope = Factory.Services.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        }


        [Fact]
        public void Creates()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-23");
            var newDto = new AppRatingDto
            {
                UserId = -23,
                Score = 1,
                Comment = "New test comment"
            };

            var result = controller.Create(newDto).Result;

            result.ShouldNotBeNull();
            var okObjectResult = result.ShouldBeOfType<OkObjectResult>();
            okObjectResult.StatusCode.ShouldBe(200);

            var storedEntity = _dbContext.AppRatings.FirstOrDefault(r => r.Comment == "New test comment");
            storedEntity.ShouldNotBeNull();
            storedEntity.Id.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Updates()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-21");
            var updatedDto = new AppRatingDto
            {
                Id = -1,
                Score = 1,
                Comment = "Pera's comment has been edited."
            };

            var result = controller.Update(updatedDto.Id, updatedDto);

            result.ShouldNotBeNull();

            var okObjectResult = result.Result.ShouldBeOfType<OkObjectResult>();
            okObjectResult.StatusCode.ShouldBe(200);

            var updatedEntity = okObjectResult.Value.ShouldBeOfType<AppRatingDto>();
            updatedEntity.Id.ShouldBe(-1);
            updatedEntity.Comment.ShouldBe("Pera's comment has been edited.");
            updatedEntity.Score.ShouldBe(1);

            var storedEntity = _dbContext.AppRatings.Find(-1L);
            storedEntity.ShouldNotBeNull();
            storedEntity.Comment.ShouldBe("Pera's comment has been edited.");
            storedEntity.UpdatedAt.ShouldNotBeNull();
        }

        [Fact]
        public void Deletes()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-22");
            long idToDelete = -2;

            var result = controller.Delete(idToDelete);

            result.ShouldNotBeNull();
            result.ShouldBeOfType<OkResult>();

            var storedEntity = _dbContext.AppRatings.Find(idToDelete);
            storedEntity.ShouldBeNull();
        }

        [Fact]
        public void Retrieves_my_rating()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-21");

            var result = controller.GetMyRating().Result;

            result.ShouldNotBeNull();
            var okObjectResult = result.ShouldBeOfType<OkObjectResult>();
            var myRatings = okObjectResult.Value.ShouldBeOfType<System.Collections.Generic.List<AppRatingDto>>();

            myRatings.Count.ShouldBe(1);
        }


        [Fact]
        public void Retrieves_all_as_admin()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAdminController(scope);

            var actionResult = controller.GetAll(0, 0).Result;

            actionResult.ShouldNotBeNull();
            var okObjectResult = actionResult.ShouldBeOfType<OkObjectResult>();
            var result = okObjectResult.Value.ShouldBeOfType<PagedResult<AppRatingDto>>();

            result.Results.Count.ShouldBe(2);
            result.TotalCount.ShouldBe(2);
        }



        private static Explorer.API.Controllers.Tourist.AppRatingController CreateTouristController(IServiceScope scope, string personId)
        {
            return new Explorer.API.Controllers.Tourist.AppRatingController(
                scope.ServiceProvider.GetRequiredService<API.Public.IAppRatingService>())
            {
                ControllerContext = BuildContext(personId)
            };
        }

        private static Explorer.API.Controllers.Administrator.AppRatingController CreateAdminController(IServiceScope scope)
        {
            return new Explorer.API.Controllers.Administrator.AppRatingController(
                scope.ServiceProvider.GetRequiredService<API.Public.IAppRatingService>())
            {
                ControllerContext = BuildContext("-1")
            };
        }
    }
}