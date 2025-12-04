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
        Comment Get(long id);
        Comment Update(Comment comment);
        void Delete(long id);
        List<Comment> GetAll();
        List<Comment> GetByBlogPost(long blogPostId);
    }
}
