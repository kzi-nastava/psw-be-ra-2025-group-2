using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Blog.API.Dtos;

namespace Explorer.Blog.API.Public
{
    public interface IBlogPostService
    {
        Task<BlogPostDto> CreateAsync(CreateBlogPostDto dto);
        Task<BlogPostDto?> GetByIdAsync(long id);
        Task<IEnumerable<BlogPostDto>> GetByAuthorAsync(long authorId);
        Task UpdateAsync(long id, UpdateBlogPostDto dto);
    }
}
