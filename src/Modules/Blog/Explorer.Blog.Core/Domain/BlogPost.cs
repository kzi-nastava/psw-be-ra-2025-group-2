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

        public BlogState State { get; private set; }

        public DateTime? LastModifiedAt { get; private set; }

        private BlogPost() { }
        public BlogPost(string title, string description, long authorId, List<BlogImage>? images = null, bool skipAuthorValidation = false)
        {
            Title = title;
            Description = description;
            AuthorId = authorId;
            CreatedAt = DateTime.UtcNow;
            State = BlogState.Draft;

            if (images != null)
            {
                _images.Clear();
                _images.AddRange(images);
            }

            Validate(skipAuthorValidation);
        }

        private void Validate(bool skipAuthorValidation = false)
        {
            if (string.IsNullOrWhiteSpace(Title))
                throw new ArgumentException("Title cannot be empty.");

            if (string.IsNullOrWhiteSpace(Description))
                throw new ArgumentException("Description cannot be empty.");

            if (!skipAuthorValidation && AuthorId == 0)
                throw new ArgumentException("Invalid author.");
        }
        public void Edit(string title, string description, List<BlogImage> images)
        {
            if (State != BlogState.Draft)
                throw new InvalidOperationException("Blog can be edited only while in Draft state.");
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty.");

            Title = title;
            Description = description;

            _images.Clear();
            _images.AddRange(images);
        }

        public void EditDescription(string description)
        {
            if (State != BlogState.Published)
                throw new InvalidOperationException("Only published blogs can update description.");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty.");

            Description = description;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void Publish()
        {
            if (State != BlogState.Draft)
                throw new InvalidOperationException("Only draft blogs can be published.");

            State = BlogState.Published;
        }

        public void Archive()
        {
            if (State != BlogState.Published)
                throw new InvalidOperationException("Only published blogs can be archived.");

            State = BlogState.Archived;
        }

    }
}
