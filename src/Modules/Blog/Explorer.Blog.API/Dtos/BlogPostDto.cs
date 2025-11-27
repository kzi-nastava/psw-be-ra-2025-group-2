using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Dtos
{
    public class BlogPostDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } // Markdown text
        public DateTime CreatedAt { get; set; }
        public long AuthorId { get; set; }
        public List<string> ImageUrls { get; set; } = new();
    }
}
