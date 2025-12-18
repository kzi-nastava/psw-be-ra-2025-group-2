using Explorer.API.Controllers.Stakeholders;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos.Quizzes;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Tests.Integration
{
    public class QuizQueryTests : BaseStakeholdersIntegrationTest
    {
        public QuizQueryTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Gets_full_for_author()
        {
            using(var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-11", "author");

                var results = controller.GetPagedByAuthor(-11, 1, 5).Result.ShouldBeOfType<OkObjectResult>();
                var quizzes = results.Value.ShouldBeOfType<PagedResult<QuizDto>>();
                quizzes.Results.Count.ShouldBe(5);

                foreach(var quiz in quizzes.Results)
                {
                    quiz.AvailableOptions[0].Explanation.ShouldNotBeNull();
                    quiz.AvailableOptions[0].IsCorrect.ShouldNotBeNull();
                }
            }
        }

        [Fact]
        public void Gets_full_for_author_fails_invalid_access_rights()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-12", "author");

                var results = controller.GetPagedByAuthor(-11, 1, 5).Result.ShouldBeOfType<ForbidResult>();
            }
        }

        [Fact]
        public void Gets_published_for_tourist()
        {
            using(var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-21", "tourist");

                var results = controller.GetPagedPublishedBlanks(1, 0).Result.ShouldBeOfType<OkObjectResult>();
                var quizzes = results.Value.ShouldBeOfType<PagedResult<QuizDto>>();
                quizzes.Results.Count.ShouldBe(3);

                foreach (var quiz in quizzes.Results)
                {
                    quiz.AvailableOptions[0].Explanation.ShouldBeNull();
                    quiz.AvailableOptions[0].IsCorrect.ShouldBeNull();
                }
            }
        }

        [Fact]
        public void Gets_published_for_tourist_by_author()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-21", "tourist");

                var results = controller.GetPagedPublishedBlanksByAuthor(-11, 1, 0).Result.ShouldBeOfType<OkObjectResult>();
                var quizzes = results.Value.ShouldBeOfType<PagedResult<QuizDto>>();
                quizzes.Results.Count.ShouldBe(2);

                foreach (var quiz in quizzes.Results)
                {
                    quiz.AvailableOptions[0].Explanation.ShouldBeNull();
                    quiz.AvailableOptions[0].IsCorrect.ShouldBeNull();
                }

                results = controller.GetPagedPublishedBlanksByAuthor(-12, 1, 0).Result.ShouldBeOfType<OkObjectResult>();
                quizzes = results.Value.ShouldBeOfType<PagedResult<QuizDto>>();
                quizzes.Results.Count.ShouldBe(1);

                foreach (var quiz in quizzes.Results)
                {
                    quiz.AvailableOptions[0].Explanation.ShouldBeNull();
                    quiz.AvailableOptions[0].IsCorrect.ShouldBeNull();
                }
            }
        }

        [Fact]
        public void GetsCorrectPageNumbers()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-11", "author");

                controller.GetPageCountByAuthor(-12, 5).Result.ShouldBeOfType<ForbidResult>();

                controller.GetPageCountByAuthor(-11, 6).Result.ShouldBeOfType<OkObjectResult>().Value.ShouldBeOfType<int>().ShouldBe(1);
                controller.GetPageCountByAuthor(-11, 5).Result.ShouldBeOfType<OkObjectResult>().Value.ShouldBeOfType<int>().ShouldBe(1);
                controller.GetPageCountByAuthor(-11, 2).Result.ShouldBeOfType<OkObjectResult>().Value.ShouldBeOfType<int>().ShouldBe(3);
                controller.GetPageCountByAuthor(-11, 1).Result.ShouldBeOfType<OkObjectResult>().Value.ShouldBeOfType<int>().ShouldBe(5);
                controller.GetPageCountByAuthor(-11, 0).Result.ShouldBeOfType<OkObjectResult>().Value.ShouldBeOfType<int>().ShouldBe(1);

                controller.GetPageCountPublished(1).Result.ShouldBeOfType<OkObjectResult>().Value.ShouldBeOfType<int>().ShouldBe(3);
                controller.GetPageCountPublished(2).Result.ShouldBeOfType<OkObjectResult>().Value.ShouldBeOfType<int>().ShouldBe(2);
                controller.GetPageCountPublished(10).Result.ShouldBeOfType<OkObjectResult>().Value.ShouldBeOfType<int>().ShouldBe(1);

                controller.GetPageCountPublishedByAuthor(-11, 1).Result.ShouldBeOfType<OkObjectResult>().Value.ShouldBeOfType<int>().ShouldBe(2);
                controller.GetPageCountPublishedByAuthor(-11, 5).Result.ShouldBeOfType<OkObjectResult>().Value.ShouldBeOfType<int>().ShouldBe(1);
                controller.GetPageCountPublishedByAuthor(-12, 1).Result.ShouldBeOfType<OkObjectResult>().Value.ShouldBeOfType<int>().ShouldBe(1);
                controller.GetPageCountPublishedByAuthor(-12, 10).Result.ShouldBeOfType<OkObjectResult>().Value.ShouldBeOfType<int>().ShouldBe(1);
            }
        }

        private static QuizController CreateControllerWithRole(IServiceScope scope, string userId, string role)
        {
            return new QuizController(
                scope.ServiceProvider.GetRequiredService<IQuizService>())
            {
                ControllerContext = BuildContextWithRole(userId, role)
            };
        }

        private static ControllerContext BuildContextWithRole(string id, string role)
        {
            var claims = new List<Claim>
            {
                new Claim("id", id),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "test");
            var user = new ClaimsPrincipal(identity);

            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
    }
}
