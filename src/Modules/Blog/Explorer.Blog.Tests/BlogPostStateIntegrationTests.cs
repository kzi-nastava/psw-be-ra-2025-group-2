using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.API.Controllers.Tourist.Blog;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Blog.Tests
{
    [Collection("Sequential")]
    public class BlogPostStateIntegrationTests : BaseBlogIntegrationTest
    {
        public BlogPostStateIntegrationTests(BlogTestFactory factory) : base(factory) { }

        private async Task<long> CreateAndPublishBlogWithVotes(
            BlogPostController ownerController,
            IServiceScope scope,
            List<(string userId, int voteValue)> existingVotes = null)
        {
            var create = new CreateBlogPostDto
            {
                Title = "Published Blog for Voting",
                Description = "This blog is published and ready for votes.",
                ImageUrls = new List<string>()
            };

            var createResult = await ownerController.Create(create);

            // Ekstrahuj BlogPostDto
            BlogPostDto? blog = null;
            if (createResult.Result is CreatedAtActionResult createdResult)
            {
                blog = createdResult.Value as BlogPostDto;
            }
            else if (createResult.Result is OkObjectResult okResult)
            {
                blog = okResult.Value as BlogPostDto;
            }
            else if (createResult.Value != null)
            {
                blog = createResult.Value;
            }

            blog.ShouldNotBeNull("Failed to extract BlogPostDto from Create result");

            // Publikuj blog
            await ownerController.Publish(blog.Id);

            // Dodaj postojeće glasove ako su prosleđeni
            if (existingVotes != null && existingVotes.Any())
            {
                foreach (var (userId, voteValue) in existingVotes)
                {
                    var voterController = CreateController(scope, userId);
                    await voterController.Vote(blog.Id, voteValue);
                }
            }

            return blog.Id;
        }

        [Fact]
        public async Task Upvote_existing_published_blog()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var ownerController = CreateController(scope, "-21");

            // Kreiraj blog i dodaj postojeća 3 glasa: 2 upvote, 1 downvote
            var existingVotes = new List<(string userId, int voteValue)>
    {
        ("-22", 1),   // upvote
        ("-23", 1),   // upvote
        ("-11", -1)   // downvote
    };

            var blogId = await CreateAndPublishBlogWithVotes(ownerController, scope, existingVotes);

            // Kreiraj novi kontroler za korisnika -21 koji će glasati
            var voterController = CreateController(scope, "-21");

            // Act: dodaj upvote korisnika -21
            var voteResult = await voterController.Vote(blogId, 1);
            VoteResultDto? result = ExtractVoteResult(voteResult);

            // Assert
            result.ShouldNotBeNull();
            result.Score.ShouldBe(2);        
            result.UpvoteCount.ShouldBe(3);  
            result.DownvoteCount.ShouldBe(1);
            result.UserVote.ShouldBe(1);
        }

        [Fact]
        public async Task Remove_vote_resets_user_vote()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var ownerController = CreateController(scope, "-21");

            // Kreiraj blog i dodaj postojeća 3 glasa, uključujući glas od -22
            var existingVotes = new List<(string userId, int voteValue)>
            {
                ("-22", 1),   // upvote - ovaj će biti uklonjen
                ("-23", 1),   // upvote
                ("-11", -1)   // downvote
            };

            var blogId = await CreateAndPublishBlogWithVotes(ownerController, scope, existingVotes);

            // Kreiraj kontroler za korisnika -22 koji će ukloniti svoj glas
            var voterController = CreateController(scope, "-22");

            // Act: korisnik -22 uklanja svoj glas
            var voteResult = await voterController.RemoveVote(blogId);
            VoteResultDto? result = ExtractVoteResult(voteResult);

            // Assert
            result.ShouldNotBeNull();
            result.UserVote.ShouldBeNull();
            result.UpvoteCount.ShouldBe(1);  // Ostao samo -23
            result.DownvoteCount.ShouldBe(1); // Ostao -11
            result.Score.ShouldBe(0);         // 1 upvote - 1 downvote = 0
        }
        [Fact]
        public async Task Blog_becomes_closed_after_receiving_enough_downvotes()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var ownerController = CreateController(scope, "-21");
            var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

            // Kreiraj blog bez glasova
            var blogId = await CreateAndPublishBlogWithVotes(ownerController, scope, null);

            // Act: Dodaj 11 downvotes da score padne ispod -10
            for (int i = 1; i <= 11; i++)
            {
                var voterController = CreateController(scope, $"-{100 + i}");
                await voterController.Vote(blogId, -1);
            }

            // Assert: Proveri da je blog zatvoren
            var blog = await dbContext.BlogPosts.FindAsync(blogId);
            blog.State.ShouldBe(BlogState.Closed);
        }

        [Fact]
        public async Task Closed_blog_cannot_receive_new_votes()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var ownerController = CreateController(scope, "-21");

            var blogId = await CreateAndPublishBlogWithVotes(ownerController, scope, null);

            // Zatvori blog (dodaj 11 downvotes)
            for (int i = 1; i <= 11; i++)
            {
                var voterController = CreateController(scope, $"-{100 + i}");
                await voterController.Vote(blogId, -1);
            }

            // Act: Pokušaj glasati na zatvorenom blogu
            var newVoterController = CreateController(scope, "-500");
            var voteResult = await newVoterController.Vote(blogId, 1);

            // Assert: Očekuj BadRequest
            voteResult.Result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Blog_becomes_active_when_score_exceeds_100()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var ownerController = CreateController(scope, "-21");
            var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

            var blogId = await CreateAndPublishBlogWithVotes(ownerController, scope, null);

            // Act: Dodaj 101 upvotes
            for (int i = 1; i <= 101; i++)
            {
                var voterController = CreateController(scope, $"-{200 + i}");
                await voterController.Vote(blogId, 1);
            }

            // Assert
            var blog = await dbContext.BlogPosts.FindAsync(blogId);
            blog.State.ShouldBe(BlogState.Active);
        }

        [Fact]
        public async Task Blog_becomes_famous_when_score_exceeds_500()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var ownerController = CreateController(scope, "-21");
            var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

            var blogId = await CreateAndPublishBlogWithVotes(ownerController, scope, null);

            // Act: Dodaj 501 upvotes
            for (int i = 1; i <= 501; i++)
            {
                var voterController = CreateController(scope, $"-{300 + i}");
                await voterController.Vote(blogId, 1);
            }

            // Assert
            var blog = await dbContext.BlogPosts.FindAsync(blogId);
            blog.State.ShouldBe(BlogState.Famous);
        }

        [Fact]
        public async Task Active_blog_can_still_receive_votes_and_comments()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var ownerController = CreateController(scope, "-21");
            var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

            var blogId = await CreateAndPublishBlogWithVotes(ownerController, scope, null);

            // Postavi blog u Active status
            for (int i = 1; i <= 101; i++)
            {
                var voterController = CreateController(scope, $"-{200 + i}");
                await voterController.Vote(blogId, 1);
            }

            // Act: Pokušaj glasati na Active blogu
            var newVoterController = CreateController(scope, "-999");
            var voteResult = await newVoterController.Vote(blogId, 1);
            var result = ExtractVoteResult(voteResult);

            // Assert
            result.ShouldNotBeNull();
            result.UserVote.ShouldBe(1);

            var blog = await dbContext.BlogPosts.FindAsync(blogId);
            blog.State.ShouldBe(BlogState.Active); // ili Famous ako je prešlo 500
        }

        private static VoteResultDto ExtractVoteResult(ActionResult<VoteResultDto> actionResult)
        {
            if (actionResult.Result is OkObjectResult okResult)
                return okResult.Value as VoteResultDto;

            if (actionResult.Result is BadRequestObjectResult badRequest)
                throw new Exception("BadRequest returned: " + System.Text.Json.JsonSerializer.Serialize(badRequest.Value));

            if (actionResult.Result is NotFoundObjectResult notFound)
                throw new Exception("NotFound returned: " + System.Text.Json.JsonSerializer.Serialize(notFound.Value));

            if (actionResult.Result is NotFoundResult)
                throw new Exception("NotFound returned (no body)");

            if (actionResult.Result != null)
                throw new Exception($"Unexpected result type: {actionResult.Result.GetType().Name}");

            if (actionResult.Value != null)
                return actionResult.Value;

            throw new Exception("Both Result and Value are null");
        }

        private static BlogPostController CreateController(IServiceScope scope, string personId)
        {
            var claims = new System.Security.Claims.Claim[]
            {
                new System.Security.Claims.Claim("personId", personId),
                new System.Security.Claims.Claim("id", personId),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, personId),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "Test User")
            };

            var identity = new System.Security.Claims.ClaimsIdentity(claims, "TestAuthScheme");
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);

            return new BlogPostController(
                scope.ServiceProvider.GetRequiredService<IBlogPostService>())
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
                    {
                        User = principal
                    }
                }
            };
        }
    }
}
