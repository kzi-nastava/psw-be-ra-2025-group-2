using Explorer.Blog.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Public
{
    public interface ICommentService
    {
        
            
                // Osnovne CRUD operacije
                CommentDto Create(CommentDto commentDto);
                CommentDto Update(CommentDto commentDto, long commentId);
                void Delete(long id);

                // Preuzimanje komentara
                List<CommentDto> GetAll();
                CommentDto Get(long id);
                List<CommentDto> GetByBlogPost(long blogPostId);
            
        

    }

}