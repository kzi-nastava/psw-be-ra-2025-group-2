using System;
using Explorer.Blog.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Blog.Tests.TestData
{
    public class VoteValueUnitTests
    {
        [Fact]
        public void Upvote_has_value_of_one()
        {
            // Act
            var upvote = VoteValue.Upvote;

            // Assert
            upvote.Value.ShouldBe(1);
        }

        [Fact]
        public void Downvote_has_value_of_minus_one()
        {
            // Act
            var downvote = VoteValue.Downvote;

            // Assert
            downvote.Value.ShouldBe(-1);
        }

        [Fact]
        public void VoteValue_equality_works_correctly()
        {
            // Arrange
            var upvote1 = VoteValue.Upvote;
            var upvote2 = VoteValue.Upvote;
            var downvote = VoteValue.Downvote;

            // Assert
            upvote1.Equals(upvote2).ShouldBeTrue();
            upvote1.Equals(downvote).ShouldBeFalse();
        }
    }
}