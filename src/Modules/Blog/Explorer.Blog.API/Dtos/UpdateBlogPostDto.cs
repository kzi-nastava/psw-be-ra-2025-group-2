using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Dtos
{
    public class UpdateBlogPostDto
    {
        public required string Title { get; set; }
        public  required string Description { get; set; }
        public List<string>? ImageUrls { get; set; }
    }
}
