using System.Net;
using System.Net.Http.Json;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Blog.Tests;

[Collection("Sequential")]
public class CommentTests : BaseBlogIntegrationTest
{
    public CommentTests(BlogTestFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Creates_comment_successfully()
    {
        // Arrange
        var client = Factory.CreateClient();
        var request = new CommentDto
        {
            UserId = -21,
            BlogPostId = -1,
            Text = "Ovo je novi test komentar."
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/blogpost/comments", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<CommentDto>();
        created.ShouldNotBeNull();
        created.Text.ShouldBe("Ovo je novi test komentar.");
        created.UserId.ShouldBe(-21);
        created.BlogPostId.ShouldBe(-1);

        // Verify in database
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var comment = db.Comments.FirstOrDefault(c => c.Id == created.Id);
        comment.ShouldNotBeNull();
        comment.Text.ShouldBe("Ovo je novi test komentar.");
    }

    [Fact]
    public async Task Create_fails_on_empty_text()
    {
        // Arrange
        var client = Factory.CreateClient();
        var request = new CommentDto
        {
            UserId = -21,
            BlogPostId = -1,
            Text = ""
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/blogpost/comments", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_fails_on_invalid_blog_post_id()
    {
        // Arrange
        var client = Factory.CreateClient();
        var request = new CommentDto
        {
            UserId = -21,
            BlogPostId = 9999, // Ne postoji
            Text = "Komentar na nepostojeci post"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/blogpost/comments", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_all_comments_successfully()
    {
        // Arrange
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/blogpost/comments");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<List<CommentDto>>();
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Get_comment_by_id_successfully()
    {
        // Arrange
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/blogpost/comments/-1");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<CommentDto>();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
    }

    [Fact]
    public async Task Get_comment_fails_on_invalid_id()
    {
        // Arrange
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/blogpost/comments/9999");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_comments_by_blog_post_successfully()
    {
        // Arrange
        var client = Factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/blogpost/comments/by-post/-1");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<List<CommentDto>>();
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
        result.ShouldAllBe(c => c.BlogPostId == -1);
    }

    [Fact]
    public async Task Updates_comment_successfully()
    {
        // Arrange
        var client = Factory.CreateClient();
        var updateRequest = new CommentDto
        {
            Text = "Ažurirani tekst komentara."
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/blogpost/comments/-3", updateRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var updated = await response.Content.ReadFromJsonAsync<CommentDto>();
        updated.ShouldNotBeNull();
        updated.Text.ShouldBe("Ažurirani tekst komentara.");

        // Verify in database
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var comment = db.Comments.FirstOrDefault(c => c.Id == -3);
        comment.ShouldNotBeNull();
        comment.Text.ShouldBe("Ažurirani tekst komentara.");
        comment.LastModifiedAt.ShouldNotBeNull();
    }

    [Fact]
    public async Task Update_fails_on_invalid_id()
    {
        // Arrange
        var client = Factory.CreateClient();
        var updateRequest = new CommentDto
        {
            Text = "Ne postoji."
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/blogpost/comments/9999", updateRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_fails_on_empty_text()
    {
        // Arrange
        var client = Factory.CreateClient();
        var updateRequest = new CommentDto
        {
            Text = ""
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/blogpost/comments/-3", updateRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_fails_when_not_owner()
    {
        // Arrange
        var client = Factory.CreateClient();
        // Pretpostavljamo da je komentar -1 kreirao user -21, 
        // a pokušavamo da update-ujemo kao user -1
        var updateRequest = new CommentDto
        {
            UserId = -1, // Različit user
            Text = "Pokušaj neovlašćenog update-a."
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/blogpost/comments/-1", updateRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Deletes_comment_successfully()
    {
        // Arrange
        var client = Factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/api/blogpost/comments/-8");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // Verify in database
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var comment = db.Comments.FirstOrDefault(c => c.Id == -8);
        comment.ShouldBeNull();
    }

    [Fact]
    public async Task Delete_fails_on_invalid_id()
    {
        // Arrange
        var client = Factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/api/blogpost/comments/9999");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_fails_when_not_owner()
    {
        // Arrange
        var client = Factory.CreateClient();
        // Pretpostavljamo da je komentar -2 kreirao user -21,
        // a pokušavamo da delete-ujemo kao user -1

        // Act
        var response = await client.DeleteAsync("/api/blogpost/comments/-2");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_fails_when_edit_window_expired()
    {
        // Arrange
        var client = Factory.CreateClient();

        // Trebalo bi da imaš komentar u bazi koji je stariji od 15 minuta
        // ili da kreiraš takav komentar u setup-u testa

        // Act
        var response = await client.DeleteAsync("/api/blogpost/comments/-10");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}