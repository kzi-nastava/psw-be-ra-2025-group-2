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
        CommentDto Create(long id, string text); // Autor iz IUsernameProvider, vreme je DateTime.Now
        void Edit(long id, DateTime createdAt, string newText); // Pronalazi po autoru i vremenu
        void Delete(long id, DateTime createdAt); // Pronalazi po autoru i vremenu 
    }


}