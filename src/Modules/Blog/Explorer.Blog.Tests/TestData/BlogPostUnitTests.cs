using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Blog.Core.Domain;
using Shouldly;

namespace Explorer.Blog.Tests.TestData
{
    public class BlogPostUnitTests
    {
        [Theory]
        [InlineData("Valid Title", "Valid Description", -21, true)] // Success
        [InlineData("", "Valid Description", -21, false)] // Empty title fails
        [InlineData("Valid Title", "", -21, false)] // Empty description fails
        public void Creates_blog_with_validation(string title, string description, long authorId, bool shouldSucceed)
        {
            if (shouldSucceed)
            {
                // Act
                var blog = new BlogPost(title, description, authorId, null, true);

                // Assert
                blog.Title.ShouldBe(title);
                blog.Description.ShouldBe(description);
                blog.AuthorId.ShouldBe(authorId);
                blog.State.ShouldBe(BlogState.Draft);
            }
            else
            {
                // Act & Assert
                Should.Throw<ArgumentException>(() =>
                    new BlogPost(title, description, authorId, null, true)
                );
            }
        }

        [Theory]
        [InlineData(BlogState.Draft, "New Title", "New Desc", true)] // Draft can be edited
        [InlineData(BlogState.Published, "New Title", "New Desc", false)] // Published cannot be edited
        public void Edits_blog_based_on_state(BlogState initialState, string newTitle, string newDescription, bool shouldSucceed)
        {
            // Arrange
            var blog = new BlogPost("Original Title", "Original Description", -21, null, true);
            if (initialState == BlogState.Published)
                blog.Publish();

            // Act & Assert
            if (shouldSucceed)
            {
                blog.Edit(newTitle, newDescription, new List<BlogImage>());
                blog.Title.ShouldBe(newTitle);
                blog.Description.ShouldBe(newDescription);
            }
            else
            {
                Should.Throw<InvalidOperationException>(() =>
                    blog.Edit(newTitle, newDescription, new List<BlogImage>())
                );
            }
        }

        [Theory]
        [InlineData("", "Valid Description", false)] // Empty title
        [InlineData("Valid Title", "", false)] // Empty description
        [InlineData("Valid Title", "Valid Description", true)] // Valid edit
        public void Edit_validates_input(string title, string description, bool shouldSucceed)
        {
            // Arrange
            var blog = new BlogPost("Original", "Original Desc", -21, null, true);

            // Act & Assert
            if (shouldSucceed)
            {
                blog.Edit(title, description, new List<BlogImage>());
                blog.Title.ShouldBe(title);
                blog.Description.ShouldBe(description);
            }
            else
            {
                Should.Throw<ArgumentException>(() =>
                    blog.Edit(title, description, new List<BlogImage>())
                );
            }
        }

        [Theory]
        [InlineData(BlogState.Draft, BlogState.Published, true)] // Draft → Published succeeds
        [InlineData(BlogState.Published, BlogState.Published, false)] // Published → Published fails
        public void Publishes_based_on_state(BlogState initialState, BlogState expectedState, bool shouldSucceed)
        {
            // Arrange
            var blog = new BlogPost("Title", "Description", -21, null, true);
            if (initialState == BlogState.Published)
                blog.Publish(); // Već published

            // Act & Assert
            if (shouldSucceed)
            {
                blog.Publish();
                blog.State.ShouldBe(expectedState);
            }
            else
            {
                Should.Throw<InvalidOperationException>(() => blog.Publish());
            }
        }

        [Fact]
        public void Edit_replaces_images()
        {
            // Arrange
            var originalImages = new List<BlogImage>
        {
            new BlogImage("old1.png"),
            new BlogImage("old2.png")
        };
            var blog = new BlogPost("Title", "Description", -21, originalImages, true);

            var newImages = new List<BlogImage>
        {
            new BlogImage("new1.png")
        };

            // Act
            blog.Edit("Title", "Description", newImages);

            // Assert
            blog.Images.Count.ShouldBe(1);
            blog.Images.ShouldNotContain(i => i.Url == "old1.png");
            blog.Images.ShouldNotContain(i => i.Url == "old2.png");
            blog.Images.ShouldContain(i => i.Url == "new1.png");
        }

        [Theory]
        [InlineData(0)] // No images
        [InlineData(1)] // One image
        [InlineData(3)] // Multiple images
        public void Creates_blog_with_varying_image_count(int imageCount)
        {
            // Arrange
            var images = Enumerable.Range(0, imageCount)
                .Select(i => new BlogImage($"image{i}.png"))
                .ToList();

            // Act
            var blog = new BlogPost("Title", "Description", -21, images, true);

            // Assert
            blog.Images.Count.ShouldBe(imageCount);
        }
    }
}
