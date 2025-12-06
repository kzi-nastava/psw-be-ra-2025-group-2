using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Blog.API.Dtos;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Blog.API.Public
{
    public interface IBlogPostService
    {
        Task<BlogPostDto> CreateAsync(CreateBlogPostDto dto, long authorId);
        Task<BlogPostDto?> GetByIdAsync(long id, long? currentUserId = null);
        Task<IEnumerable<BlogPostDto>> GetByAuthorAsync(long authorId, long? currentUserId = null);
        Task<BlogPostDto> UpdateAsync(long id, UpdateBlogPostDto dto, long userId);
        Task PublishAsync(long id, long userId);
        Task ArchiveAsync(long id, long userId);
        Task<IEnumerable<BlogPostDto>> GetVisibleBlogsAsync(long? currentUserId, int? filterStatus = null);
    }
}
