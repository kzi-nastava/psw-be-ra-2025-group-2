using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Infrastructure;
using Shouldly;
using Xunit;

namespace Explorer.Blog.Tests.TestData
{
    public class BlogPostStateUnitTests
    {
        [Theory]
        [InlineData(-11, 0, BlogState.Closed)]   // score < -10 → Closed
        [InlineData(-50, 20, BlogState.Closed)]  // score < -10 → Closed
        public void Blog_becomes_closed_when_score_too_low(int score, int comments, BlogState expected)
        {
            // Arrange
            var blog = new BlogPost("Test", "Desc", 1);
            blog.Publish();
            AddVotes(blog, score);

            // Act
            blog.UpdateStatus(comments);

            // Assert
            blog.State.ShouldBe(expected);
        }

        [Theory]
        [InlineData(101, 0, BlogState.Active)] // score > 100 → Active
        [InlineData(0, 11, BlogState.Active)]  // comments > 10 → Active
        [InlineData(150, 20, BlogState.Active)] // score & comments → Active (if not Famous)
        public void Blog_becomes_active(int score, int comments, BlogState expected)
        {
            var blog = new BlogPost("Test", "Desc", 1);
            blog.Publish();
            AddVotes(blog, score);

            blog.UpdateStatus(comments);

            blog.State.ShouldBe(expected);
        }

        [Theory]
        [InlineData(501, 0, BlogState.Famous)] // score > 500 → Famous
        [InlineData(0, 31, BlogState.Famous)]  // comments > 30 → Famous
        [InlineData(600, 50, BlogState.Famous)] // score & comments → Famous
        public void Blog_becomes_famous(int score, int comments, BlogState expected)
        {
            var blog = new BlogPost("Test", "Desc", 1);
            blog.Publish();
            AddVotes(blog, score);

            blog.UpdateStatus(comments);

            blog.State.ShouldBe(expected);
        }

        private void AddVotes(BlogPost blog, int score)
        {
            if (score == 0) return;

            int votesToAdd = Math.Abs(score);
            for (int i = 0; i < votesToAdd; i++)
            {
                blog.AddVote(1000 + i, score > 0 ? VoteValue.Upvote : VoteValue.Downvote);
            }
        }
    }
}
