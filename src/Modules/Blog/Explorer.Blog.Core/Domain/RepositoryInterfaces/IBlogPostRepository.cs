using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Blog.Core.Domain.RepositoryInterfaces
{
    public interface IBlogPostRepository
    {
        Task<BlogPost> AddAsync(BlogPost blogPost);
        Task<BlogPost?> GetByIdAsync(long id);
        Task<IEnumerable<BlogPost>> GetByAuthorAsync(long authorId);
        Task UpdateAsync(BlogPost blogPost);
        Task<IEnumerable<BlogPost>> GetAllAsync();

    }
}
