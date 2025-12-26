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
            => new GeoLocation(45.2671, 19.8335); // example coords

        private static ExperiencePoints ValidXp()
            => new ExperiencePoints(100);

        [Fact]
        public void Creates_in_draft_state()
        {
            var encounter = new Encounter(
                "Social encounter",
                "Meet locals",
                ValidLocation(),
                ValidXp(),
                EncounterType.Social
            );

            encounter.Name.ShouldBe("Social encounter");
            encounter.Description.ShouldBe("Meet locals");
            encounter.Location.ShouldNotBeNull();
            encounter.XP.ShouldNotBeNull();
            encounter.Type.ShouldBe(EncounterType.Social);
            encounter.State.ShouldBe(EncounterState.Draft);
        }

        [Fact]
        public void Creation_fails_when_required_fields_are_invalid()
        {
            Should.Throw<EntityValidationException>(() =>
                new Encounter(
                    "",
                    "Description",
                    ValidLocation(),
                    ValidXp(),
                    EncounterType.Location));

            Should.Throw<EntityValidationException>(() =>
                new Encounter(
                    "Name",
                    "",
                    ValidLocation(),
                    ValidXp(),
                    EncounterType.Location));

            Should.Throw<EntityValidationException>(() =>
                new Encounter(
                    "Name",
                    "Description",
                    null!,
                    ValidXp(),
                    EncounterType.Location));

            Should.Throw<EntityValidationException>(() =>
                new Encounter(
                    "Name",
                    "Description",
                    ValidLocation(),
                    null!,
                    EncounterType.Location));
        }

        [Fact]
        public void Updates_when_in_draft_state()
        {
            var encounter = new Encounter(
                "Old name",
                "Old description",
                ValidLocation(),
                ValidXp(),
                EncounterType.Social
            );

            var newLocation = new GeoLocation(44.7866, 20.4489);
            var newXp = new ExperiencePoints(250);

            encounter.Update(
                "New name",
                "New description",
                newLocation,
                newXp,
                EncounterType.Location
            );

            encounter.Name.ShouldBe("New name");
            encounter.Description.ShouldBe("New description");
            encounter.Location.ShouldBe(newLocation);
            encounter.XP.ShouldBe(newXp);
            encounter.Type.ShouldBe(EncounterType.Location);
        }

        [Fact]
        public void Update_fails_when_not_in_draft_state()
        {
            var encounter = new Encounter(
                "Name",
                "Description",
                ValidLocation(),
                ValidXp(),
                EncounterType.Social
            );

            encounter.MakeActive();

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
            var encounter = new Encounter(
                "Name",
                "Description",
                ValidLocation(),
                ValidXp(),
                EncounterType.Location
            );

            encounter.MakeActive();

            encounter.State.ShouldBe(EncounterState.Active);
        }

        [Fact]
        public void Cannot_activate_when_already_active()
        {
            var encounter = new Encounter(
                "Name",
                "Description",
                ValidLocation(),
                ValidXp(),
                EncounterType.Location
            );

            encounter.MakeActive();

            Should.Throw<InvalidOperationException>(encounter.MakeActive);
        }

        [Fact]
        public void Archives_only_from_active_state()
        {
            var encounter = new Encounter(
                "Name",
                "Description",
                ValidLocation(),
                ValidXp(),
                EncounterType.Location
            );

            encounter.MakeActive();
            encounter.Archive();

            encounter.State.ShouldBe(EncounterState.Archived);
        }

        [Fact]
        public void Cannot_archive_when_not_active()
        {
            var encounter = new Encounter(
                "Name",
                "Description",
                ValidLocation(),
                ValidXp(),
                EncounterType.Location
            );

            Should.Throw<InvalidOperationException>(encounter.Archive);
        }
    }

}
