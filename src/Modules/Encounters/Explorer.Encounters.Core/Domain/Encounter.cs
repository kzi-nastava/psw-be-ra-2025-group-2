using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Encounters.Core.Domain
{
    public enum EncounterState
    {
        Draft = 0,
        Active = 1,
        Archived = 2
    }

    public enum EncounterType
    {
        Social = 0,
        Location = 1,
        Miscellaneous = 2
    }
    public abstract class Encounter : AggregateRoot
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public GeoLocation Location { get; private set; }
        public ExperiencePoints XP { get; private set; }
        public EncounterState State { get; private set; }
        public EncounterType Type { get; private set; }
        protected Encounter() { }

        protected Encounter(string name, string description, GeoLocation location, ExperiencePoints xp, EncounterType type)
        {
            Name = name;
            Description = description;
            Location = location;
            XP = xp;
            Type = type;
            State = EncounterState.Draft;
        }

        public void Update(string name, string description, GeoLocation location, ExperiencePoints xp, EncounterType type)
        {
            if (State != EncounterState.Draft)
                throw new InvalidOperationException("Cannot update an encounter which is not in a draft state.");

            Name = name;
            Description = description;
            Location = location;
            XP = xp;
            Type = type;
            Validate();
        }

        public void MakeActive()
        {
            if (State == EncounterState.Active)
                throw new InvalidOperationException("Cannot activate an encounter which is already active.");
            State = EncounterState.Active;
        }

        public void Archive()
        {
            if (State != EncounterState.Active)
                throw new InvalidOperationException("Cannot archive an encounter which is not active.");
            State = EncounterState.Archived;
        }

        public bool IsActive() => State == EncounterState.Active;

        protected virtual void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) throw new EntityValidationException("Name cannot be empty.");
            if (string.IsNullOrWhiteSpace(Description)) throw new EntityValidationException("Description cannot be empty.");
            if (Location == null) throw new EntityValidationException("Location cannot be null.");
            if (XP == null) throw new EntityValidationException("XP cannot be null.");
        }
    }

    public class SocialEncounter : Encounter
    {
        public int RequiredPeople { get; private set; }
        public double Range { get; private set; }
        protected SocialEncounter() { }

        public SocialEncounter(string name, string description, GeoLocation location, ExperiencePoints xp, int requiredPeople, double range)
            : base(name, description, location, xp, EncounterType.Social)
        {
            RequiredPeople = requiredPeople;
            Range = range;
            Validate();
        }

        public void UpdateSocialEncounter(int requiredPeople, double range)
        {
            if (State != EncounterState.Draft)
                throw new InvalidOperationException("Cannot update social fields when encounter is not in Draft state.");

            RequiredPeople = requiredPeople;
            Range = range;
            Validate();
        }

        protected override void Validate()
        {
            base.Validate();
            if (RequiredPeople < 1) throw new EntityValidationException("Required people must be at least 1.");
            if (Range <= 0) throw new EntityValidationException("Range must be greater than 0.");
        }
    }

    public class HiddenLocationEncounter : Encounter
    {
        public string ImageUrl { get; private set; }
        public GeoLocation ImageLocation { get; private set; }
        public double DistanceTreshold { get; private set; }

        protected HiddenLocationEncounter() { }

        public HiddenLocationEncounter(string name, string description, GeoLocation location, ExperiencePoints xp, string imageUrl, GeoLocation imageLocation, double distanceTreshold)
            : base(name, description, location, xp, EncounterType.Location)
        {
            ImageUrl = imageUrl;
            ImageLocation = imageLocation;
            DistanceTreshold = distanceTreshold;
            Validate();
        }

        public void UpdateHiddenLocationFields(string imageUrl, GeoLocation imageLocation, double distanceTreshold)
        {
            if (State != EncounterState.Draft)
                throw new InvalidOperationException("Cannot update hidden-location fields when encounter is not in Draft state.");

            ImageUrl = imageUrl;
            ImageLocation = imageLocation;
            DistanceTreshold = distanceTreshold;
            Validate();
        }

        protected override void Validate()
        {
            base.Validate();
            if (string.IsNullOrWhiteSpace(ImageUrl)) throw new EntityValidationException("Image URL cannot be empty.");
            if (ImageLocation == null) throw new EntityValidationException("Image location cannot be null.");
            if (DistanceTreshold <= 0) throw new EntityValidationException("Distance treshold must be greater than 0.");
        }

        public void UpdateHiddenLocation(string imageUrl, GeoLocation imageLocation, double distanceTreshold)
        {
            ImageUrl = imageUrl;
            ImageLocation = imageLocation;
            DistanceTreshold = distanceTreshold;
            Validate();
        }
    }
    public class MiscEncounter : Encounter
    {
        protected MiscEncounter() { }

        public MiscEncounter(string name, string description, GeoLocation location, ExperiencePoints xp)
            : base(name, description, location, xp, EncounterType.Miscellaneous)
        {
        }
    }
}