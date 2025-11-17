using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.API.Public;

namespace Explorer.Blog.Core.UseCases
{
    public class BlogPostService : IBlogPostService
    {
        private readonly IBlogPostRepository _repository;
        private readonly IMapper _mapper;

        public BlogPostService(IBlogPostRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<BlogPostDto> CreateAsync(CreateBlogPostDto dto)
        {
            var images = dto.ImageUrls?.Select(url => new BlogImage(url)).ToList();
            var blogPost = new BlogPost(dto.Title, dto.Description, dto.AuthorId, images, skipAuthorValidation: dto.AuthorId < 0);
            var created = await _repository.AddAsync(blogPost);
            return _mapper.Map<BlogPostDto>(created);
        }

        public async Task<BlogPostDto?> GetByIdAsync(long id)
        {
            var blog = await _repository.GetByIdAsync(id);
            return blog == null ? null : _mapper.Map<BlogPostDto>(blog);
        }

        public async Task<IEnumerable<BlogPostDto>> GetByAuthorAsync(long authorId)
        {
            var blogs = await _repository.GetByAuthorAsync(authorId);
            return _mapper.Map<IEnumerable<BlogPostDto>>(blogs);
        }

        public async Task UpdateAsync(long id, UpdateBlogPostDto dto)
        {
            var blog = await _repository.GetByIdAsync(id);
            if (blog == null)
                throw new Exception("Blog not found");

            blog.UpdateTitle(dto.Title);
            blog.UpdateDescription(dto.Description);

            if (dto.ImageUrls != null)
            {
                var images = dto.ImageUrls.Select(url => new BlogImage(url)).ToList();
                blog.ReplaceImages(images);
            }

            await _repository.UpdateAsync(blog);
        }
    }
}
