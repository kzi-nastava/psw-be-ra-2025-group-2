using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.Core.Domain.RepositoryInterfaces
{
    public interface ICommentRepository
    {
        Comment Create(Comment comment);
        Comment Get(string username); 
        Comment Update(Comment comment); 
        void Delete(string username); 
        List<Comment> GetAll();
    }
}
