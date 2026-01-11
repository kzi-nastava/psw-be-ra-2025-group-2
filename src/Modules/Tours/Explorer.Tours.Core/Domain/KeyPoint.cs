using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class KeyPoint : Entity, IKeyPointInfo
    {
        public int OrdinalNo { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string SecretText { get; private set; }
        public string ImageUrl { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public long AuthorId { get; private set; }
        public bool IsPublic { get; private set; } = false;

        public long? EncounterId { get; private set; }
        public bool IsEncounterRequired { get; private set; }

        private KeyPoint()
        {
            Name = string.Empty;
            Description = string.Empty;
            SecretText = string.Empty;
            ImageUrl = string.Empty;
            IsEncounterRequired = false; 
        }
        public KeyPoint(
            int ordinalNo,
            string name,
            string description,
            string secretText,
            string imageUrl,
            double latitude,
            double longitude,
            long authorId,
            long? encounterId = null,
            bool isEncounterRequired = false)
        {
            OrdinalNo = ordinalNo;
            Name = name;
            Description = description;
            SecretText = secretText;
            ImageUrl = imageUrl;
            Latitude = latitude;
            Longitude = longitude;
            AuthorId = authorId;
            IsPublic = false;
            EncounterId = encounterId;
            IsEncounterRequired = isEncounterRequired;

            Validate();
        }

        public KeyPoint(
            int ordinalNo,
            string name,
            string description,
            string secretText,
            string imageUrl,
            double latitude,
            double longitude)
            : this(ordinalNo, name, description, secretText, imageUrl, latitude, longitude, 0, null, false)
        {
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Name is required");

            if (Latitude < -90 || Latitude > 90)
                throw new ArgumentOutOfRangeException(nameof(Latitude));

            if (Longitude < -180 || Longitude > 180)
                throw new ArgumentOutOfRangeException(nameof(Longitude));

            if (IsEncounterRequired && !EncounterId.HasValue)
                throw new ArgumentException("Cannot mark encounter as required when no encounter is assigned.");
        }

        public void Update(string name, string description, string secretText, string imageUrl,
                          double latitude, double longitude, long? encounterId, bool isEncounterRequired)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required");

            Name = name;
            Description = description;
            SecretText = secretText ?? string.Empty;
            ImageUrl = imageUrl;
            Latitude = latitude;
            Longitude = longitude;
            EncounterId = encounterId;
            IsEncounterRequired = isEncounterRequired;

            Validate();
        }

        public bool HasMandatoryEncounter() => IsEncounterRequired && EncounterId.HasValue;

        public void MakePublic(long? requestId = null)
        {
            IsPublic = true;
        }

        internal void SetOrdinalNo(int ordinalNo)
        {
            if (ordinalNo <= 0)
                throw new ArgumentOutOfRangeException(nameof(ordinalNo), "Ordinal number must be positive.");

            OrdinalNo = ordinalNo;
        }

    }
}