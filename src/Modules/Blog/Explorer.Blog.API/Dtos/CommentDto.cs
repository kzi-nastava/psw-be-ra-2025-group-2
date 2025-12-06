using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Dtos
{
    public class CommentDto
    {
        public long Id { get; set; }
        public long BlogPostId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; } // Ime korisnika koji je ostavio komentar
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
       
    }

}

