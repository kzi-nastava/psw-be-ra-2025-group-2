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
            AuthorId = -1,
            ImageUrls = new List<string> { "new_img_1.png", "new_img_2.png" }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/blogpost", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<BlogPostDto>();
        created.ShouldNotBeNull();
        created.ImageUrls.Count.ShouldBe(2);

        // Verify in database
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var blogPost = db.BlogPosts
            .Include(b => b.Images)
            .FirstOrDefault(p => p.Id == created.Id);

        blogPost.ShouldNotBeNull();
        blogPost.Title.ShouldBe("New Valid Post Title");
        blogPost.Images.Count.ShouldBe(2);
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
            AuthorId = -1,
            ImageUrls = new List<string>()
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/blogpost", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_blog_posts_by_author_successfully()
    {
        // Arrange
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/blogpost/author/-21");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<List<BlogPostDto>>();
        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);
        result.ShouldAllBe(p => p.AuthorId == -21);
    }

    [Fact]
    public async Task Updates_blog_post_successfully_and_replaces_images()
    {
        // Arrange
        var client = Factory.CreateClient();
        var updateRequest = new UpdateBlogPostDto
        {
            Title = "Updated Title OK",
            Description = "New description content.",
            ImageUrls = new List<string> { "new_image_a.png", "new_image_b.png", "new_image_c.png" }
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/blogpost/-2", updateRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // Verify in database
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var blogPost = db.BlogPosts
            .Include(b => b.Images)
            .FirstOrDefault(p => p.Id == -2);

        blogPost.ShouldNotBeNull();
        blogPost.Title.ShouldBe("Updated Title OK");
        blogPost.Description.ShouldBe("New description content.");
        blogPost.Images.Count.ShouldBe(3);
        blogPost.Images.ShouldContain(i => i.Url == "new_image_a.png");
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

    [Fact]
    public async Task Update_fails_on_empty_description()
    {
        // Arrange
        var client = Factory.CreateClient();
        var updateRequest = new UpdateBlogPostDto
        {
            Title = "Valid Title",
            Description = "",
            ImageUrls = new List<string>()
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/blogpost/-2", updateRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}