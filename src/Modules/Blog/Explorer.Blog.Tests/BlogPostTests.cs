using System.Net;
using System.Net.Http.Json;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Blog.Tests;

[Collection("Sequential")]
public class BlogPostTests : BaseBlogIntegrationTest
{
    public BlogPostTests(BlogTestFactory factory) : base(factory)
    { 
    }

    
    [Fact]
    public async Task Creates_blog_post_successfully()
    {
        // Arrange
        var client = Factory.CreateClient();
        var request = new CreateBlogPostDto
        {
            Title = "New Valid Post Title",
            Description = "Description with images.",
            ImageUrls = new List<string> { "new_img_1.png", "new_img_2.png" }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/blogpost", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<BlogPostDto>();
        created.ShouldNotBeNull();
        created.ImageUrls.Count.ShouldBe(2);
        created.State.ShouldBe(0); // Draft state
        created.AuthorId.ShouldNotBe(0); // AuthorId postavljen iz tokena

        // Verify in database
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var blogPost = db.BlogPosts
            .Include(b => b.Images)
            .FirstOrDefault(p => p.Id == created.Id);

        blogPost.ShouldNotBeNull();
        blogPost.Title.ShouldBe("New Valid Post Title");
        blogPost.Images.Count.ShouldBe(2);
        blogPost.State.ShouldBe(Core.Domain.BlogState.Draft);
    }

    [Fact]
    public async Task Create_fails_on_empty_title()
    {
        // Arrange
        var client = Factory.CreateClient();
        var request = new CreateBlogPostDto
        {
            Title = "",
            Description = "Valid Desc",
            ImageUrls = new List<string>()
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/blogpost", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task Create_fails_on_empty_description()
    {
        // Arrange
        var client = Factory.CreateClient();
        var request = new CreateBlogPostDto
        {
            Title = "Valid Title",
            Description = "", // Prazna description
            ImageUrls = new List<string>()
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/blogpost", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_fails_on_invalid_id()
    {
        // Arrange
        var client = Factory.CreateClient();
        var updateRequest = new UpdateBlogPostDto
        {
            Title = "Non-existent",
            Description = "Should fail",
            ImageUrls = new List<string>()
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/blogpost/9999", updateRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

}