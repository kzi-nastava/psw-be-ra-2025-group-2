using Explorer.API.Controllers.Tourist.Blog;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Blog.Tests;

[Collection("Sequential")]
public class BlogPostVoteTests : BaseBlogIntegrationTest
{
    public BlogPostVoteTests(BlogTestFactory factory) : base(factory) { }

    private async Task<long> CreateAndPublishBlog(BlogPostController controller)
    {
        var create = new CreateBlogPostDto
        {
            Title = "Test voting",
            Description = "Voting desc",
            ImageUrls = new List<string>()
        };

        var createResult = await controller.Create(create);

        // Ekstrahuj BlogPostDto iz CreatedAtActionResult
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

        blog.ShouldNotBeNull($"Failed to extract BlogPostDto from Create result");

        await controller.Publish(blog.Id);

        return blog.Id;
    }

    [Fact]
    public async Task Upvote_published_blog_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();
        var blogId = await CreateAndPublishBlog(controller);

        // Act
        var voteResult = await controller.Vote(blogId, 1);

        // Ekstrahuj VoteResultDto
        VoteResultDto? result = null;
        if (voteResult.Result is OkObjectResult okResult)
        {
            result = okResult.Value as VoteResultDto;
        }
        else if (voteResult.Value != null)
        {
            result = voteResult.Value;
        }

        // Assert - Response
        result.ShouldNotBeNull();
        result.Score.ShouldBe(1);
        result.UpvoteCount.ShouldBe(1);
        result.DownvoteCount.ShouldBe(0);
        result.UserVote.ShouldBe(1);

        // Assert - Database
        var blog = dbContext.BlogPosts.Include(b => b.Votes).First(b => b.Id == blogId);
        blog.GetScore().ShouldBe(1);
        blog.Votes.Count.ShouldBe(1);
    }

    [Fact]
    public async Task Changing_vote_updates_counts_correctly()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");
        var blogId = await CreateAndPublishBlog(controller);

        // Act
        await controller.Vote(blogId, 1);
        var voteResult = await controller.Vote(blogId, -1);

        VoteResultDto? result = null;
        if (voteResult.Result is OkObjectResult okResult)
        {
            result = okResult.Value as VoteResultDto;
        }
        else if (voteResult.Value != null)
        {
            result = voteResult.Value;
        }

        // Assert
        result.ShouldNotBeNull();
        result.UpvoteCount.ShouldBe(0);
        result.DownvoteCount.ShouldBe(1);
        result.UserVote.ShouldBe(-1);
        result.Score.ShouldBe(-1);
    }

    [Fact]
    public async Task Same_vote_twice_does_not_change_score()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");
        var blogId = await CreateAndPublishBlog(controller);

        // Act
        await controller.Vote(blogId, 1);
        var voteResult = await controller.Vote(blogId, 1);

        VoteResultDto? result = null;
        if (voteResult.Result is OkObjectResult okResult)
        {
            result = okResult.Value as VoteResultDto;
        }
        else if (voteResult.Value != null)
        {
            result = voteResult.Value;
        }

        // Assert
        result.ShouldNotBeNull();
        result.Score.ShouldBe(1);
        result.UpvoteCount.ShouldBe(1);
        result.DownvoteCount.ShouldBe(0);
        result.UserVote.ShouldBe(1);
    }

    [Fact]
    public async Task Remove_vote_resets_user_vote()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");
        var blogId = await CreateAndPublishBlog(controller);

        // Act
        await controller.Vote(blogId, 1);
        var voteResult = await controller.RemoveVote(blogId);

        VoteResultDto? result = null;
        if (voteResult.Result is OkObjectResult okResult)
        {
            result = okResult.Value as VoteResultDto;
        }
        else if (voteResult.Value != null)
        {
            result = voteResult.Value;
        }

        // Assert
        result.ShouldNotBeNull();
        result.UserVote.ShouldBe(null);
        result.Score.ShouldBe(0);
        result.UpvoteCount.ShouldBe(0);
        result.DownvoteCount.ShouldBe(0);
    }

    [Fact]
    public async Task Cannot_vote_on_draft_blog()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");

        var create = new CreateBlogPostDto
        {
            Title = "draft",
            Description = "draft desc",
            ImageUrls = new List<string>()
        };
        var createResult = await controller.Create(create);

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

        blog.ShouldNotBeNull();

        // Act
        var voteResult = await controller.Vote(blog.Id, 1);

        // Assert
        voteResult.Result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Voting_on_nonexistent_blog_returns_NotFound()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");

        // Act
        var voteResult = await controller.Vote(999999, 1);

        // Assert
        voteResult.Result.ShouldBeOfType<NotFoundObjectResult>();
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

        // VAŽNO: authenticationType mora biti postavljen da bi IsAuthenticated bio true
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