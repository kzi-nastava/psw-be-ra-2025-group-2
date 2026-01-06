using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class KeyPoint : Entity, IKeyPointInfo
    {
        public int OrdinalNo { get; private set; }
        public string Name { get; init; }
        public string Description { get; init; }
        public string SecretText { get; init; }
        public string ImageUrl { get; init; }
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public long AuthorId { get; private set; }
        public bool IsPublic { get; private set; } = false;

        public long? EncounterId { get; init; }

        private KeyPoint() { }

        public KeyPoint(
            int ordinalNo,
            string name,
            string description,
            string secretText,
            string imageUrl,
            double latitude,
            double longitude,
            long authorId,
            long? encounterId = null) 
        {
            OrdinalNo = ordinalNo;
            Name = name;
            Description = description;
            SecretText = secretText ?? string.Empty;
            ImageUrl = imageUrl ?? string.Empty;
            Latitude = latitude;
            Longitude = longitude;
            AuthorId = authorId;
            IsPublic = false;
            EncounterId = encounterId;

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
             : this(ordinalNo, name, description, secretText, imageUrl, latitude, longitude, 0, null)
        {
        }

        public void MarkAsPublic()
        {
            IsPublic = true;
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Name is required");

            if (string.IsNullOrWhiteSpace(Description))
                throw new ArgumentException("Description is required");

            if (Latitude < -90 || Latitude > 90)
                throw new ArgumentOutOfRangeException(nameof(Latitude));

            if (Longitude < -180 || Longitude > 180)
                throw new ArgumentOutOfRangeException(nameof(Longitude));
        }

        public void Update(string name, string description, string secretText, string imageUrl, double latitude, double longitude, long? encounterId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required");

            typeof(KeyPoint).GetProperty(nameof(Name))!.SetValue(this, name);
            typeof(KeyPoint).GetProperty(nameof(Description))!.SetValue(this, description);
            typeof(KeyPoint).GetProperty(nameof(SecretText))!.SetValue(this, secretText ?? string.Empty);
            typeof(KeyPoint).GetProperty(nameof(ImageUrl))!.SetValue(this, imageUrl ?? string.Empty);
            typeof(KeyPoint).GetProperty(nameof(Latitude))!.SetValue(this, latitude);
            typeof(KeyPoint).GetProperty(nameof(Longitude))!.SetValue(this, longitude);
            typeof(KeyPoint).GetProperty(nameof(EncounterId))!.SetValue(this, encounterId);

            Validate();
        }

        public void SetOrdinalNo(int newOrdinal)
        {
            if (newOrdinal < 1)
                throw new ArgumentOutOfRangeException(nameof(newOrdinal));

            OrdinalNo = newOrdinal;
        }
    }
}