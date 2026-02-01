using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Encounters.Core.Domain;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Tests.Unit
{
    public class EncountersUnitTests
    {
        private static GeoLocation ValidLocation()
            => new GeoLocation(45.2671, 19.8335);

        private static ExperiencePoints ValidXp()
            => new ExperiencePoints(100);

        [Fact]
        public void Creates_social_encounter_in_draft_state()
        {
            // Arrange & Act
            var encounter = new SocialEncounter(
                "Social encounter",
                "Meet locals",
                ValidLocation(),
                ValidXp(),
                5,
                10.5
            );

            // Assert - Base properties
            encounter.Name.ShouldBe("Social encounter");
            encounter.Description.ShouldBe("Meet locals");
            encounter.Location.ShouldNotBeNull();
            encounter.XP.ShouldNotBeNull();
            encounter.Type.ShouldBe(EncounterType.Social);
            encounter.State.ShouldBe(EncounterState.Draft);

            // Assert - Specific properties
            encounter.RequiredPeople.ShouldBe(5);
            encounter.Range.ShouldBe(10.5);
        }

        [Fact]
        public void Creates_hidden_location_encounter_in_draft_state()
        {
            // Arrange & Act
            var encounter = new HiddenLocationEncounter(
                "Hidden gem",
                "Find the spot",
                ValidLocation(),
                ValidXp(),
                "http://image.url/test.jpg",
                new GeoLocation(45.0, 19.0),
                5.0
            );

            // Assert
            encounter.Type.ShouldBe(EncounterType.Location);
            encounter.ImageUrl.ShouldBe("http://image.url/test.jpg");
            encounter.DistanceTreshold.ShouldBe(5.0);
        }

        [Fact]
        public void Creates_misc_encounter_in_draft_state()
        {
            // Arrange & Act
            var encounter = new MiscEncounter(
                "Do pushups",
                "20 pushups",
                ValidLocation(),
                ValidXp()
            );

            // Assert
            encounter.Type.ShouldBe(EncounterType.Miscellaneous);
            encounter.State.ShouldBe(EncounterState.Draft);
        }

        [Fact]
        public void Social_creation_fails_when_parameters_invalid()
        {
            /*Should.Throw<EntityValidationException>(() =>
                new SocialEncounter(
                    "Name", "Desc", ValidLocation(), ValidXp(),
                    0, 10));

            Should.Throw<EntityValidationException>(() =>
                new SocialEncounter(
                    "Name", "Desc", ValidLocation(), ValidXp(),
                    5, -5));*/
        }

        [Fact]
        public void Hidden_location_creation_fails_when_missing_image_data()
        {
            /*Should.Throw<EntityValidationException>(() =>
                new HiddenLocationEncounter(
                    "Name", "Desc", ValidLocation(), ValidXp(),
                    "", ValidLocation(), 5));

            Should.Throw<EntityValidationException>(() =>
                new HiddenLocationEncounter(
                    "Name", "Desc", ValidLocation(), ValidXp(),
                    "url", null!, 5));*/
        }

        [Fact]
        public void Updates_when_in_draft_state()
        {
            // Arrange (Misc Encounter)
            var encounter = new MiscEncounter(
                "Old name",
                "Old description",
                ValidLocation(),
                ValidXp()
            );

            var newLocation = new GeoLocation(44.7866, 20.4489);
            var newXp = new ExperiencePoints(250);

            // Act
            encounter.Update(
                "New name",
                "New description",
                newLocation,
                newXp,
                EncounterType.Miscellaneous
            );

            // Assert
            encounter.Name.ShouldBe("New name");
            encounter.Description.ShouldBe("New description");
            encounter.Location.ShouldBe(newLocation);
            encounter.XP.ShouldBe(newXp);
        }

        [Fact]
        public void Update_fails_when_not_in_draft_state()
        {
            // Arrange
            var encounter = new MiscEncounter(
                "Name",
                "Description",
                ValidLocation(),
                ValidXp()
            );

            encounter.MakeActive();

            // Act & Assert
            Should.Throw<InvalidOperationException>(() =>
                encounter.Update(
                    "New",
                    "New",
                    ValidLocation(),
                    ValidXp(),
                    EncounterType.Miscellaneous));
        }

        [Fact]
        public void Activates_from_draft()
        {
            var encounter = new MiscEncounter(
                "Name",
                "Description",
                ValidLocation(),
                ValidXp()
            );

            encounter.MakeActive();

            encounter.State.ShouldBe(EncounterState.Active);
        }

        [Fact]
        public void Cannot_activate_when_already_active()
        {
            var encounter = new MiscEncounter(
                "Name",
                "Description",
                ValidLocation(),
                ValidXp()
            );

            encounter.MakeActive();

            Should.Throw<InvalidOperationException>(encounter.MakeActive);
        }

        [Fact]
        public void Archives_only_from_active_state()
        {
            var encounter = new MiscEncounter(
                "Name",
                "Description",
                ValidLocation(),
                ValidXp()
            );

            encounter.MakeActive();
            encounter.Archive();

            encounter.State.ShouldBe(EncounterState.Archived);
        }

        [Fact]
        public void Cannot_archive_when_not_active()
        {
            var encounter = new MiscEncounter(
                "Name",
                "Description",
                ValidLocation(),
                ValidXp()
            );

            Should.Throw<InvalidOperationException>(encounter.Archive);
        }

        [Fact]
        public void Sets_rewards_correctly()
        {
            // Arrange
            var encounter = new MiscEncounter(
                "Reward Encounter",
                "Description",
                ValidLocation(),
                ValidXp()
            );

            // Act
            encounter.SetRewards(100, 200);

            // Assert
            encounter.FavoriteTourId.ShouldBe(100);
            encounter.FavoriteBlogId.ShouldBe(200);
        }
    }
}