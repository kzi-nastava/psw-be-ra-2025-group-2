using Explorer.API.Controllers;
using Explorer.API.Controllers.Stakeholders;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos.Quizzes;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.Quizzes;
using Explorer.Stakeholders.Infrastructure.Database;
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
    [Collection("Sequential")]
    public class QuizCommandTests : BaseStakeholdersIntegrationTest
    {
        public QuizCommandTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates()
        {
            using(var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-11", "author");
                var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
                var newQuizDto = new QuizDto
                {
                    AuthorId = -69,
                    QuestionText = "What is the capital of Serbia?",
                    AvailableOptions = new List<QuizOptionDto>
                    {
                        new QuizOptionDto {Ordinal = 0, OptionText = "Belgrade", Explanation = "Correct.", IsCorrect = true},
                        new QuizOptionDto {Ordinal = 0, OptionText = "Novi Sad", Explanation = "Incorrect.", IsCorrect = false}
                    },
                    IsPublished = true
                };

                var result = controller.Create(newQuizDto).Result.ShouldBeOfType<OkObjectResult>();
                var quiz = result.Value.ShouldBeOfType<QuizDto>();
                quiz.ShouldNotBeNull();
                quiz.Id.ShouldNotBe(0);
                quiz.AuthorId.ShouldBe(-11);
                quiz.QuestionText.ShouldBe("What is the capital of Serbia?");
                quiz.AvailableOptions.Count.ShouldBe(2);
                quiz.IsPublished.ShouldBeFalse();

                var storedEntity = dbContext.Quizzes.Find(quiz.Id);

                storedEntity.ShouldNotBeNull();
                storedEntity.AuthorId.ShouldBe(-11);
                storedEntity.QuestionText.ShouldBe("What is the capital of Serbia?");
                storedEntity.AvailableOptions.Count.ShouldBe(2);
                storedEntity.IsPublished.ShouldBeFalse();

                Assert.True(storedEntity.AvailableOptions[0].Equals(new QuizOption(1, "Belgrade", "Correct.", true)));
                Assert.True(storedEntity.AvailableOptions[1].Equals(new QuizOption(2, "Novi Sad", "Incorrect.", false)));
            }
        }

        [Fact]
        public void Updates()
        {
            using(var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-11", "author");
                var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

                var result = controller.GetPagedByAuthor(-11, 1, 5).Result.ShouldBeOfType<OkObjectResult>();
                var quizzes = result.Value.ShouldBeOfType<PagedResult<QuizDto>>();

                quizzes.Results.Count.ShouldBe(5);
                var quiz = quizzes.Results.Where(r => r.Id == -3).FirstOrDefault();

                quiz.ShouldNotBeNull();
                quiz.QuestionText.ShouldBe("Serbia is located on which continent?");
                quiz.IsPublished.ShouldBeFalse();
                quiz.AvailableOptions.Count.ShouldBe(3);
                quiz.AvailableOptions[0].OptionText.ShouldBe("Europe");
                quiz.AvailableOptions[0].IsCorrect.ShouldNotBeNull().ShouldBeTrue();

                var updatedQuiz = new QuizDto
                {
                    Id = quiz.Id,
                    AuthorId = -11,
                    QuestionText = "What is the capital of Hungary?",
                    AvailableOptions = new List<QuizOptionDto>
                    {
                        new QuizOptionDto {Ordinal = 0, OptionText = "Budapest", Explanation = "Correct.", IsCorrect = true},
                        new QuizOptionDto {Ordinal = 0, OptionText = "Timisoara", Explanation = "Incorrect.", IsCorrect = false}
                    },
                    IsPublished = true
                };

                result = controller.Update(quiz.Id, updatedQuiz).Result.ShouldBeOfType<OkObjectResult>();
                quiz = result.Value.ShouldBeOfType<QuizDto>();

                quiz.ShouldNotBeNull();
                quiz.QuestionText.ShouldBe("What is the capital of Hungary?");
                quiz.AvailableOptions.Count.ShouldBe(2);
                quiz.IsPublished.ShouldBeFalse();
                quiz.AvailableOptions[0].OptionText.ShouldBe("Budapest");
                quiz.AvailableOptions[0].IsCorrect.ShouldNotBeNull().ShouldBeTrue();

                var entity = dbContext.Quizzes.Find(quiz.Id);

                entity.ShouldNotBeNull();
                entity.AuthorId.ShouldBe(quiz.AuthorId);
                entity.QuestionText.ShouldBe("What is the capital of Hungary?");
                entity.AvailableOptions.Count.ShouldBe(2);
                entity.IsPublished.ShouldBeFalse();

                Assert.True(entity.AvailableOptions[0].Equals(new QuizOption(1, "Budapest", "Correct.", true)));
                Assert.True(entity.AvailableOptions[1].Equals(new QuizOption(2, "Timisoara", "Incorrect.", false)));
            }
        }

        [Fact]
        public void Update_fails_invalid_data()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-11", "author");
                var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

                var result = controller.GetPagedByAuthor(-11, 1, 5).Result.ShouldBeOfType<OkObjectResult>();
                var quizzes = result.Value.ShouldBeOfType<PagedResult<QuizDto>>();

                quizzes.Results.Count.ShouldBe(5);
                var quiz = quizzes.Results.Where(r => r.Id == -3).FirstOrDefault();

                quiz.ShouldNotBeNull();
                quiz.QuestionText.ShouldBe("Serbia is located on which continent?");
                quiz.IsPublished.ShouldBeFalse();
                quiz.AvailableOptions.Count.ShouldBe(3);
                quiz.AvailableOptions[0].OptionText.ShouldBe("Europe");
                quiz.AvailableOptions[0].IsCorrect.ShouldNotBeNull().ShouldBeTrue();

                var updatedQuiz = new QuizDto
                {
                    Id = -100,  // Nepostojeci id kviza
                    AuthorId = -11,
                    QuestionText = "What is the capital of Hungary?",
                    AvailableOptions = new List<QuizOptionDto>
                    {
                        new QuizOptionDto {Ordinal = 0, OptionText = "Budapest", Explanation = "Correct.", IsCorrect = true},
                        new QuizOptionDto {Ordinal = 0, OptionText = "Timisoara", Explanation = "Incorrect.", IsCorrect = false}
                    },
                    IsPublished = true
                };

                controller.Update(quiz.Id, updatedQuiz).Result.ShouldBeOfType<BadRequestResult>();
            }
        }

        [Fact]
        public void Update_fails_published()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-11", "author");
                var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

                var result = controller.GetPagedByAuthor(-11, 1, 5).Result.ShouldBeOfType<OkObjectResult>();
                var quizzes = result.Value.ShouldBeOfType<PagedResult<QuizDto>>();

                quizzes.Results.Count.ShouldBe(5);
                var quiz = quizzes.Results.Where(r => r.Id == -2).FirstOrDefault();

                quiz.ShouldNotBeNull();
                quiz.QuestionText.ShouldBe("Which river flows through Belgrade?");
                quiz.IsPublished.ShouldBeTrue();
                quiz.AvailableOptions.Count.ShouldBe(3);
                quiz.AvailableOptions[0].OptionText.ShouldBe("Morava");
                quiz.AvailableOptions[0].IsCorrect.ShouldNotBeNull().ShouldBeFalse();

                var updatedQuiz = new QuizDto
                {
                    Id = quiz.Id,
                    AuthorId = -11,
                    QuestionText = "What is the capital of Hungary?",
                    AvailableOptions = new List<QuizOptionDto>
                    {
                        new QuizOptionDto {Ordinal = 0, OptionText = "Budapest", Explanation = "Correct.", IsCorrect = true},
                        new QuizOptionDto {Ordinal = 0, OptionText = "Timisoara", Explanation = "Incorrect.", IsCorrect = false}
                    },
                    IsPublished = true
                };

                Should.Throw<InvalidOperationException>(() => controller.Update(quiz.Id, updatedQuiz));
            }
        }

        [Fact]
        public void Publishes()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-11", "author");
                var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

                var result = controller.GetPagedByAuthor(-11, 1, 5).Result.ShouldBeOfType<OkObjectResult>();
                var quizzes = result.Value.ShouldBeOfType<PagedResult<QuizDto>>();

                quizzes.Results.Count.ShouldBe(5);
                var quiz = quizzes.Results.Where(r => r.Id == -5).FirstOrDefault();

                quiz.ShouldNotBeNull();
                quiz.QuestionText.ShouldBe("Which mountain is the highest in Serbia?");
                quiz.IsPublished.ShouldBeFalse();
                quiz.AvailableOptions.Count.ShouldBe(3);
                quiz.AvailableOptions[0].OptionText.ShouldBe("Midžor");
                quiz.AvailableOptions[0].IsCorrect.ShouldNotBeNull().ShouldBeTrue();

                controller.Publish(quiz.Id).ShouldBeOfType<OkResult>();

                var entity = dbContext.Quizzes.Find(quiz.Id);

                entity.ShouldNotBeNull();
                entity.QuestionText.ShouldBe("Which mountain is the highest in Serbia?");
                entity.IsPublished.ShouldBeTrue();
                entity.AvailableOptions.Count.ShouldBe(3);
            }
        }

        [Fact]
        public void Deletes()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-11", "author");
                var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

                controller.Delete(-1L).ShouldBeOfType<NoContentResult>();

                controller.Delete(-6L).ShouldBeOfType<ForbidResult>();

                dbContext.Quizzes.Find(-1L).ShouldBeNull();
                dbContext.Quizzes.Find(-6L).ShouldNotBeNull();
            }
        }

        [Fact]
        public void Submits()
        {
            using(var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-21", "tourist");
                var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

                var submission = new QuizSubmissionDto
                {
                    TouristId = -21,
                    QuizId = -6,
                    Answers = new List<QuizAnswerDto>
                    {
                        new QuizAnswerDto
                        {
                            Ordinal = 1,
                            IsMarkedTrue = true
                        },
                        new QuizAnswerDto
                        {
                            Ordinal = 2,
                            IsMarkedTrue = false
                        },
                        new QuizAnswerDto
                        {
                            Ordinal = 3,
                            IsMarkedTrue = false
                        },
                    }
                };

                var result = controller.Answer(-6, submission).Result.ShouldBeOfType<OkObjectResult>();
                var quiz = result.Value.ShouldBeOfType<QuizDto>();

                quiz.AvailableOptions[0].Explanation.ShouldNotBeNull();
                quiz.AvailableOptions[0].IsCorrect.ShouldNotBeNull();
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
