using System.Collections.Generic;
using System.Linq;
using Explorer.Stakeholders.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Stakeholders.Tests.Unit
{
    public class ClubBadgeTests
    {
        [Fact]
        public void Awards_badges_for_each_500xp_milestone()
        {
            // Arrange
            var club = new Club(
                name: "XP Club",
                description: "Test club",
                ownerId: 1,
                imageUrls: new List<string> { "img.jpg" });

            // Act
            var added = club.AwardMissingBadges(totalXp: 1200);

            // Assert
            added.ShouldBe(2); // 500 i 1000
            club.Badges
                .Select(b => b.MilestoneXp)
                .OrderBy(x => x)
                .ShouldBe(new[] { 500, 1000 });
        }

        [Fact]
        public void Does_not_duplicate_existing_badges()
        {
            // Arrange
            var club = new Club(
                name: "XP Club",
                description: "Test club",
                ownerId: 1,
                imageUrls: new List<string> { "img.jpg" });

            // Act
            club.AwardMissingBadges(1200);   // 500, 1000
            var addedAgain = club.AwardMissingBadges(1500); // samo 1500

            // Assert
            addedAgain.ShouldBe(1);
            club.Badges
                .Select(b => b.MilestoneXp)
                .OrderBy(x => x)
                .ShouldBe(new[] { 500, 1000, 1500 });
        }

        [Fact]
        public void Does_not_award_badges_when_total_xp_is_below_first_milestone()
        {
            // Arrange
            var club = new Club(
                name: "XP Club",
                description: "Test club",
                ownerId: 1,
                imageUrls: new List<string> { "img.jpg" });

            // Act
            var added = club.AwardMissingBadges(300);

            // Assert
            added.ShouldBe(0);
            club.Badges.ShouldBeEmpty();
        }

        [Fact]
        public void Throws_when_step_xp_is_invalid()
        {
            // Arrange
            var club = new Club(
                name: "XP Club",
                description: "Test club",
                ownerId: 1,
                imageUrls: new List<string> { "img.jpg" });

            // Act & Assert
            Should.Throw<System.ArgumentException>(() =>
                club.AwardMissingBadges(1000, stepXp: 0));
        }
    }
}
