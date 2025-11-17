using System.Net;
using System.Net.Http.Json;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Infrastructure.Database;
using Explorer.Blog.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Explorer.Blog.Tests;

public class BlogPostTests : BaseBlogIntegrationTest
{
    public BlogPostTests(BlogTestFactory factory) : base(factory) { }

    [Fact]
    public async Task Creates_blog_post_successfully()
    {// Arrange
        var client = Factory.CreateClient();

        var request = new CreateBlogPostDto
        {
            Title = "Test Title",
            Description = "Test Desc",
            AuthorId = -1, // samo neka vrednost, ne mora postojati Author
            ImageUrls = new List<string> { "img1.png" }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/blogpost", request);

        // Assert HTTP response
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Read result body
        var created = await response.Content.ReadFromJsonAsync<BlogPostDto>();
        Assert.NotNull(created);

        // Assert database changed
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        // Dohvat bloga iz baze po ID-u
        var blogPost = db.BlogPosts
            .Include(b => b.Images) // ako želiš da proveriš slike
            .FirstOrDefault(p => p.Id == created!.Id);

        Assert.NotNull(blogPost);
        Assert.Equal("Test Title", blogPost.Title);
        Assert.Equal("Test Desc", blogPost.Description);
        Assert.Equal(-1, blogPost.AuthorId);

        // Opcionalno: proveri slike
        Assert.Single(blogPost.Images);
        Assert.Equal("img1.png", blogPost.Images.First().Url);
    }
}
