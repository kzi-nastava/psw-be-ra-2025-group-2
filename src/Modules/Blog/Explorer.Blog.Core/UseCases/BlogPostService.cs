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
using Explorer.BuildingBlocks.Core.UseCases;

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

        public async Task<BlogPostDto> CreateAsync(CreateBlogPostDto dto, long authorId)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title is required");

            var images = dto.ImageUrls?.Select(url => new BlogImage(url)).ToList();
            var blogPost = new BlogPost(dto.Title, dto.Description, authorId, images);

            var created = await _repository.AddAsync(blogPost);
            return _mapper.Map<BlogPostDto>(created);
        }

        public async Task<BlogPostDto?> GetByIdAsync(long id, long? currentUserId = null)
        {
            var blog = await _repository.GetByIdAsync(id);
            if (blog == null) return null;

            if (blog.State == BlogState.Draft && currentUserId.HasValue && blog.AuthorId != currentUserId.Value)
                return null;

            var dto = _mapper.Map<BlogPostDto>(blog);
            dto.State = (int)blog.State;
            return dto;
        }

        public async Task<IEnumerable<BlogPostDto>> GetByAuthorAsync(long authorId, long? currentUserId = null)
        {
            var blogs = await _repository.GetByAuthorAsync(authorId);

            var filtered = blogs.Where(b => b.State != BlogState.Draft || b.AuthorId == currentUserId);

            var dtos = _mapper.Map<IEnumerable<BlogPostDto>>(filtered);

            foreach (var dto in dtos)
            {
                var blog = filtered.First(b => b.Id == dto.Id);
                dto.State = (int)blog.State;
            }

            return dtos;
        }

        public async Task<BlogPostDto> UpdateAsync(long id, UpdateBlogPostDto dto)
        {
            var blog = await _repository.GetByIdAsync(id);
            if (blog == null)
                throw new KeyNotFoundException("Blog not found");

            var images = dto.ImageUrls?.Select(url => new BlogImage(url)).ToList() ?? new List<BlogImage>();

            blog.Edit(dto.Title, dto.Description, images);

            await _repository.UpdateAsync(blog);

            var updatedDto = _mapper.Map<BlogPostDto>(blog);
            updatedDto.State = (int)blog.State;

            return updatedDto;
        }

        public async Task PublishAsync(long id, long userId)
        {
            var blog = await _repository.GetByIdAsync(id);
            if (blog == null)
                throw new KeyNotFoundException("Blog not found.");

            if (blog.AuthorId != userId)
                throw new UnauthorizedAccessException("You can publish only your own blogs.");

            blog.Publish();

            await _repository.UpdateAsync(blog);
        }

    }
}
