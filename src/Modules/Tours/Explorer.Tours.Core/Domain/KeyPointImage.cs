using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class KeyPointImage : Entity
    {
        public string Url { get; private set; } = string.Empty;
        public bool IsCover { get; private set; }
        public int OrderIndex { get; private set; }

        private KeyPointImage() { }

        public KeyPointImage(string url, bool isCover = false, int orderIndex = 0)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Url is required.", nameof(url));

            Url = url;
            IsCover = isCover;
            OrderIndex = orderIndex;
        }

        public void MarkAsCover() => IsCover = true;
        public void UnmarkAsCover() => IsCover = false;
    }
}
