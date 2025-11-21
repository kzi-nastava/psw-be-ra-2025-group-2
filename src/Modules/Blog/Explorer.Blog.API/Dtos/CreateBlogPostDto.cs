using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Dtos
{
    public class CreateBlogPostDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public long AuthorId { get; set; }
        public List<string>? ImageUrls { get; set; }
    }
}
