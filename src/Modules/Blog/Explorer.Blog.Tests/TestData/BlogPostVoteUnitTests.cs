using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.Blog.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Blog.Tests.TestData
{
    public class BlogPostVoteUnitTests
    {
        [Fact]
        public void AddVote_adds_upvote_successfully()
        {
            // Arrange
            var blog = new BlogPost("Title", "Description", 1, null, true);
            blog.Publish(); 

            // Act
            blog.AddVote(userId: 100, VoteValue.Upvote);

            // Assert
            blog.GetScore().ShouldBe(1);
            blog.GetUpvoteCount().ShouldBe(1);
            blog.GetDownvoteCount().ShouldBe(0);
            blog.GetUserVote(100).ShouldNotBeNull();
            blog.GetUserVote(100)!.Value.ShouldBe(1);
        }

        [Fact]
        public void AddVote_adds_downvote_successfully()
        {
            // Arrange
            var blog = new BlogPost("Title", "Description", 1, null, true);
            blog.Publish();

            // Act
            blog.AddVote(userId: 100, VoteValue.Downvote);

            // Assert
            blog.GetScore().ShouldBe(-1);
            blog.GetUpvoteCount().ShouldBe(0);
            blog.GetDownvoteCount().ShouldBe(1);
            blog.GetUserVote(100)!.Value.ShouldBe(-1);
        }

        [Fact]
        public void AddVote_throws_when_blog_not_published()
        {
            // Arrange
            var blog = new BlogPost("Title", "Description", 1, null, true);
            // Blog je u Draft stanju

            // Act & Assert
            Should.Throw<InvalidOperationException>(() => blog.AddVote(userId: 100, VoteValue.Upvote) ).Message.ShouldContain("published");
        }

        [Fact]
        public void AddVote_throws_when_blog_archived()
        {
            // Arrange
            var blog = new BlogPost("Title", "Description", 1, null, true);
            blog.Publish();
            blog.Archive();

            // Act & Assert
            Should.Throw<InvalidOperationException>(() => blog.AddVote(userId: 100, VoteValue.Upvote));
        }

        [Fact]
        public void AddVote_changes_existing_vote_from_upvote_to_downvote()
        {
            // Arrange
            var blog = new BlogPost("Title", "Description", 1, null, true);
            blog.Publish();
            blog.AddVote(userId: 100, VoteValue.Upvote);

            // Act
            blog.AddVote(userId: 100, VoteValue.Downvote);

            // Assert
            blog.GetScore().ShouldBe(-1);
            blog.GetUpvoteCount().ShouldBe(0);
            blog.GetDownvoteCount().ShouldBe(1);
            blog.GetUserVote(100)!.Value.ShouldBe(-1);
        }

        [Fact]
        public void AddVote_changes_existing_vote_from_downvote_to_upvote()
        {
            // Arrange
            var blog = new BlogPost("Title", "Description", 1, null, true);
            blog.Publish();
            blog.AddVote(userId: 100, VoteValue.Downvote);

            // Act
            blog.AddVote(userId: 100, VoteValue.Upvote);

            // Assert
            blog.GetScore().ShouldBe(1);
            blog.GetUpvoteCount().ShouldBe(1);
            blog.GetDownvoteCount().ShouldBe(0);
            blog.GetUserVote(100)!.Value.ShouldBe(1);
        }

        [Fact]
        public void AddVote_does_nothing_when_same_vote_added_again()
        {
            // Arrange
            var blog = new BlogPost("Title", "Description", 1, null, true);
            blog.Publish();
            blog.AddVote(userId: 100, VoteValue.Upvote);

            // Act
            blog.AddVote(userId: 100, VoteValue.Upvote); // Isti glas ponovo

            // Assert
            blog.GetScore().ShouldBe(1);
            blog.GetUpvoteCount().ShouldBe(1);
            blog.Votes.Count.ShouldBe(1); // Samo jedan Vote objekat
        }

        [Fact]
        public void RemoveVote_removes_existing_vote()
        {
            // Arrange
            var blog = new BlogPost("Title", "Description", 1, null, true);
            blog.Publish();
            blog.AddVote(userId: 100, VoteValue.Upvote);

            // Act
            blog.RemoveVote(userId: 100);

            // Assert
            blog.GetScore().ShouldBe(0);
            blog.GetUpvoteCount().ShouldBe(0);
            blog.GetDownvoteCount().ShouldBe(0);
            blog.GetUserVote(100).ShouldBeNull();
        }

        [Fact]
        public void RemoveVote_does_nothing_when_no_vote_exists()
        {
            // Arrange
            var blog = new BlogPost("Title", "Description", 1, null, true);
            blog.Publish();

            // Act
            blog.RemoveVote(userId: 100);

            // Assert
            blog.GetScore().ShouldBe(0);
            blog.Votes.Count.ShouldBe(0);
        }

        [Fact]
        public void Multiple_users_can_vote_on_same_blog()
        {
            // Arrange
            var blog = new BlogPost("Title", "Description", 1, null, true);
            blog.Publish();

            // Act
            blog.AddVote(userId: 100, VoteValue.Upvote);
            blog.AddVote(userId: 101, VoteValue.Upvote);
            blog.AddVote(userId: 102, VoteValue.Downvote);
            blog.AddVote(userId: 103, VoteValue.Upvote);

            // Assert
            blog.GetScore().ShouldBe(2); // 3 upvotes - 1 downvote = 2
            blog.GetUpvoteCount().ShouldBe(3);
            blog.GetDownvoteCount().ShouldBe(1);
            blog.Votes.Count.ShouldBe(4);
        }

        [Fact]
        public void GetScore_calculates_correctly_with_mixed_votes()
        {
            // Arrange
            var blog = new BlogPost("Title", "Description", 1, null, true);
            blog.Publish();

            // Act
            blog.AddVote(userId: 100, VoteValue.Upvote);    // +1 = 1
            blog.AddVote(userId: 101, VoteValue.Upvote);    // +1 = 2
            blog.AddVote(userId: 102, VoteValue.Downvote);  // -1 = 1
            blog.AddVote(userId: 103, VoteValue.Upvote);    // +1 = 2
            blog.AddVote(userId: 104, VoteValue.Downvote);  // -1 = 1
            blog.AddVote(userId: 105, VoteValue.Downvote);  // -1 = 0

            // Assert
            blog.GetScore().ShouldBe(0);
            blog.GetUpvoteCount().ShouldBe(3);
            blog.GetDownvoteCount().ShouldBe(3);
        }

        [Fact]
        public void GetUserVote_returns_null_when_user_has_not_voted()
        {
            // Arrange
            var blog = new BlogPost("Title", "Description", 1, null, true);
            blog.Publish();
            blog.AddVote(userId: 100, VoteValue.Upvote);

            // Act
            var vote = blog.GetUserVote(userId: 999);

            // Assert
            vote.ShouldBeNull();
        }

        [Fact]
        public void GetUserVote_returns_correct_vote_for_user()
        {
            // Arrange
            var blog = new BlogPost("Title", "Description", 1, null, true);
            blog.Publish();
            blog.AddVote(userId: 100, VoteValue.Upvote);
            blog.AddVote(userId: 101, VoteValue.Downvote);

            // Act & Assert
            blog.GetUserVote(userId: 100)!.Value.ShouldBe(1);
            blog.GetUserVote(userId: 101)!.Value.ShouldBe(-1);
        }

        [Theory]
        [InlineData(5, 0, 5)]   // 5 upvotes
        [InlineData(3, 2, 1)]   // 3 up, 2 down = 1
        [InlineData(2, 5, -3)]  // 2 up, 5 down = -3
        [InlineData(10, 10, 0)] // balanced
        public void GetScore_calculates_correctly_for_various_scenarios(
            int upvotes, int downvotes, int expectedScore)
        {
            // Arrange
            var blog = new BlogPost("Title", "Description", 1, null, true);
            blog.Publish();

            // Add upvotes
            for (int i = 0; i < upvotes; i++)
            {
                blog.AddVote(userId: 100 + i, VoteValue.Upvote);
            }

            // Add downvotes
            for (int i = 0; i < downvotes; i++)
            {
                blog.AddVote(userId: 200 + i, VoteValue.Downvote);
            }

            // Assert
            blog.GetScore().ShouldBe(expectedScore);
            blog.GetUpvoteCount().ShouldBe(upvotes);
            blog.GetDownvoteCount().ShouldBe(downvotes);
        }

        [Fact]
        public void Vote_immutability_test()
        {
            // Arrange
            var blog = new BlogPost("Title", "Description", 1, null, true);
            blog.Publish();
            blog.AddVote(userId: 100, VoteValue.Upvote);

            var votesBefore = blog.Votes.ToList();

            // Act - Pokušaj da promeniš vote direktno 
           
            // Jedini način je kroz agregat:
            blog.AddVote(userId: 100, VoteValue.Downvote);

            // Assert
            var votesAfter = blog.Votes.ToList();
            votesAfter.Count.ShouldBe(1);
            votesAfter[0].Value.Value.ShouldBe(-1);
        }

        [Fact]
        public void GetScore_calculates_correctly_with_multiple_votes()
        {
            // Arrange
            var blog = new BlogPost("Title", "Description", 1, null, true);
            blog.Publish();

            // Act
            blog.AddVote(userId: 100, VoteValue.Upvote);    // +1
            blog.AddVote(userId: 101, VoteValue.Upvote);    // +1
            blog.AddVote(userId: 102, VoteValue.Downvote);  // -1
            blog.AddVote(userId: 103, VoteValue.Upvote);    // +1

            // Assert
            blog.GetScore().ShouldBe(2); // 3 upvotes - 1 downvote
            blog.GetUpvoteCount().ShouldBe(3);
            blog.GetDownvoteCount().ShouldBe(1);
        }

        [Fact]
        public void GetUserVote_returns_correct_vote()
        {
            // Arrange
            var blog = new BlogPost("Title", "Description", 1, null, true);
            blog.Publish();
            blog.AddVote(userId: 100, VoteValue.Upvote);
            blog.AddVote(userId: 101, VoteValue.Downvote);

            // Act & Assert
            blog.GetUserVote(100)!.Value.ShouldBe(1);
            blog.GetUserVote(101)!.Value.ShouldBe(-1);
            blog.GetUserVote(999).ShouldBeNull();
        }
    }
}