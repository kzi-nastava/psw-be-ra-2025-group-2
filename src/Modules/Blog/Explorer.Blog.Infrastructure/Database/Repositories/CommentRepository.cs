using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Blog.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Blog.Infrastructure.Database.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly BlogContext _dbContext;

        public CommentRepository(BlogContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Comment Create(Comment comment)
        {
            _dbContext.Comments.Add(comment);
            _dbContext.SaveChanges();
            return comment;
        }

        public Comment Get(long id)
        {
            return _dbContext.Comments.Find(id);
        }

        public Comment Update(Comment comment)
        {
            _dbContext.Comments.Update(comment);
            _dbContext.SaveChanges();
            return comment;
        }

        public void Delete(long id)
        {
            var comment = Get(id);
            if (comment != null)
            {
                _dbContext.Comments.Remove(comment);
                _dbContext.SaveChanges();
            }
        }

        public List<Comment> GetAll()
        {
            return _dbContext.Comments
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
        }

        public List<Comment> GetByBlogPost(long blogPostId)
        {
            return _dbContext.Comments
                .Where(c => c.BlogPostId == blogPostId)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
        }
    }
}