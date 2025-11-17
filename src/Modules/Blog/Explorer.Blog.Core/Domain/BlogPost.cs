using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain
{
    public class BlogPost : Entity
    {
        public string Title { get; private set; }
        public string Description { get; private set; }     // Markdown text
        public DateTime CreatedAt { get; private set; }
        public long AuthorId { get; private set; }

        // Blog može imati 0, 1 ili više slika
        private readonly List<BlogImage> _images = new();
        public IReadOnlyCollection<BlogImage> Images => _images.AsReadOnly();

        private BlogPost() { }
        public BlogPost(string title, string description, long authorId, List<BlogImage>? images = null)
        {
            Title = title;
            Description = description;
            AuthorId = authorId;
            CreatedAt = DateTime.UtcNow;

            if (images != null)
            {
                _images.Clear();
                _images.AddRange(images);
            }

            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Title))
                throw new ArgumentException("Title cannot be empty.");

            if (string.IsNullOrWhiteSpace(Description))
                throw new ArgumentException("Description cannot be empty.");

            if (AuthorId <= 0)
                throw new ArgumentException("Invalid author.");
        }

        // ---------------------------
        //   UPDATE METHODS
        // ---------------------------

        public void UpdateTitle(string newTitle)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
                throw new ArgumentException("Title cannot be empty.");
            Title = newTitle;
        }

        public void UpdateDescription(string newDescription)
        {
            if (string.IsNullOrWhiteSpace(newDescription))
                throw new ArgumentException("Description cannot be empty.");
            Description = newDescription;
        }

        public void ReplaceImages(List<BlogImage> newImages)
        {
            _images.Clear();
            _images.AddRange(newImages);
        }
    }
}
