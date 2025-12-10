using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.Blog.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Blog.Tests.TestData
{
    public class BlogPostVisibilityUnitTests
    {
        [Theory]
        [InlineData(BlogState.Draft, 1, 1, true)] // Autor vidi svoj draft
        [InlineData(BlogState.Draft, 1, 2, false)] // Drugi korisnik ne vidi draft
        [InlineData(BlogState.Published, 1, 2, true)] // Svi vide published
        [InlineData(BlogState.Archived, 1, 2, true)] // Svi vide archived
        public void Users_can_see_blog_based_on_status(BlogState state, long authorId, long userId, bool shouldSee)
        {
            // Arrange
            var blog = new BlogPost("Title", "Description", authorId, null, true);

            if (state == BlogState.Published)
                blog.Publish();
            else if (state == BlogState.Archived)
            {
                blog.Publish();
                blog.Archive();
            }

            // Act
            bool canSee = blog.State switch
            {
                BlogState.Draft => blog.AuthorId == userId,
                _ => true
            };

            // Assert
            canSee.ShouldBe(shouldSee);
        }

        [Fact]
        public void Author_can_see_only_own_draft_among_multiple_blogs()
        {
            // Arrange
            var blogs = new List<BlogPost>
            {
                new BlogPost("Draft 1", "Desc", 1, null, true),
                new BlogPost("Draft 2", "Desc", 2, null, true),
                new BlogPost("Published 1", "Desc", 2, null, true)
            };

            blogs[0].State.ShouldBe(BlogState.Draft);
            blogs[1].State.ShouldBe(BlogState.Draft);
            blogs[2].Publish();

            long userId = 1;

            // Act
            var visibleBlogs = blogs.Where(b =>
                b.State != BlogState.Draft || b.AuthorId == userId
            ).ToList();

            // Assert
            visibleBlogs.Count.ShouldBe(2);
            visibleBlogs.ShouldContain(b => b.Title == "Draft 1");
            visibleBlogs.ShouldContain(b => b.Title == "Published 1");
            visibleBlogs.ShouldNotContain(b => b.Title == "Draft 2");
        }

        [Theory]
        [InlineData(BlogState.Draft, 1, true)] // Autor vidi svoj draft
        [InlineData(BlogState.Draft, 2, false)] // Drugi ne vidi draft
        [InlineData(BlogState.Published, 2, true)] // Svi vide published
        [InlineData(BlogState.Archived, 2, true)] // Svi vide archived
        public void Can_filter_blog_visibility(BlogState state, long userId, bool shouldSee)
        {
            // Arrange
            var blogs = new List<BlogPost>
            {
                new BlogPost("Draft 1", "Desc", 1, null, true),
                new BlogPost("Published 1", "Desc", 2, null, true),
                new BlogPost("Archived 1", "Desc", 2, null, true)
            };

            blogs[0].State.ShouldBe(BlogState.Draft);
            blogs[1].Publish();
            blogs[2].Publish();
            blogs[2].Archive();

            // Act
            var visibleBlogs = blogs.Where(b =>
                b.State != BlogState.Draft || b.AuthorId == userId
            ).ToList();

            // Assert
            if (state == BlogState.Draft)
                visibleBlogs.Any(b => b.State == BlogState.Draft).ShouldBe(shouldSee);
            else if (state == BlogState.Published)
                visibleBlogs.Any(b => b.State == BlogState.Published).ShouldBe(shouldSee);
            else if (state == BlogState.Archived)
                visibleBlogs.Any(b => b.State == BlogState.Archived).ShouldBe(shouldSee);
        }
    }
}
