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

        private readonly List<KeyPointImage> _images = new();
        public IReadOnlyCollection<KeyPointImage> Images => _images.AsReadOnly();

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

        public KeyPointImage AddImage(string url, bool setAsCover = false)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Url is required.", nameof(url));

            var shouldBeCover = setAsCover || _images.All(i => !i.IsCover);

            if (shouldBeCover)
                foreach (var i in _images) i.UnmarkAsCover();

            var nextOrder = _images.Count == 0 ? 0 : _images.Max(i => i.OrderIndex) + 1;

            var image = new KeyPointImage(url, shouldBeCover, nextOrder);
            _images.Add(image);

            if (shouldBeCover)
            {
                typeof(KeyPoint).GetProperty(nameof(ImageUrl))!.SetValue(this, url);
            }

            return image;
        }

        public void SetCoverImage(long imageId)
        {
            var img = _images.FirstOrDefault(i => i.Id == imageId);
            if (img == null)
                throw new ArgumentException("Image not found.", nameof(imageId));

            foreach (var i in _images) i.UnmarkAsCover();
            img.MarkAsCover();

            // sync legacy ImageUrl
            typeof(KeyPoint).GetProperty(nameof(ImageUrl))!.SetValue(this, img.Url);
        }

        public void RemoveImage(long imageId)
        {
            var img = _images.FirstOrDefault(i => i.Id == imageId);
            if (img == null) return;

            var wasCover = img.IsCover;
            _images.Remove(img);

            if (!wasCover) return;

            var newCover = _images.OrderBy(i => i.OrderIndex).FirstOrDefault();
            if (newCover != null)
            {
                foreach (var i in _images) i.UnmarkAsCover();
                newCover.MarkAsCover();
                typeof(KeyPoint).GetProperty(nameof(ImageUrl))!.SetValue(this, newCover.Url);
            }
            else
            {
                typeof(KeyPoint).GetProperty(nameof(ImageUrl))!.SetValue(this, string.Empty);
            }
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

        public void Update(
            string name,
            string description,
            string secretText,
            string imageUrl,
            double latitude,
            double longitude,
            long? encounterId)
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
