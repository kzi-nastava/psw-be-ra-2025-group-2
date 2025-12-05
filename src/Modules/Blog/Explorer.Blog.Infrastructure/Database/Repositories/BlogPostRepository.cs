using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Blog.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Blog.Infrastructure.Database.Repositories
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly BlogContext _context;

        public BlogPostRepository(BlogContext context)
        {
            _context = context;
        }

        public async Task<BlogPost> AddAsync(BlogPost blogPost)
        {
            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();
            return blogPost;
        }

        public async Task<BlogPost?> GetByIdAsync(long id)
        {
            return await _context.BlogPosts
         .Include(b => b.Images)
         .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<BlogPost>> GetByAuthorAsync(long authorId)
        {
            return await _context.BlogPosts
        .Include(b => b.Images)
        .Where(b => b.AuthorId == authorId)
        .ToListAsync();
        }

        public async Task UpdateAsync(BlogPost blogPost)
        {
            _context.BlogPosts.Update(blogPost);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await _context.BlogPosts
                .Include(b => b.Images)
                .ToListAsync();
        }
    }
}
