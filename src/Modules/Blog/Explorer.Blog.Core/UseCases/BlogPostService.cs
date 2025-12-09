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
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;


        public BlogPostService(IBlogPostRepository repository, ICommentRepository commentRepository, IMapper mapper)
        {
            _repository = repository;
            _commentRepository = commentRepository;
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

        public async Task<BlogPostDto> UpdateAsync(long id, UpdateBlogPostDto dto, long userId)
        {
            var blog = await _repository.GetByIdAsync(id);
            if (blog == null)
                throw new KeyNotFoundException("Blog not found");

            if (blog.AuthorId != userId)
                throw new UnauthorizedAccessException("You can update only your own blogs.");

            if (blog.State == BlogState.Closed)
                throw new InvalidOperationException("Closed blog cannot be edited.");


            if (blog.State == BlogState.Draft)
            {
                var images = dto.ImageUrls?.Select(url => new BlogImage(url)).ToList() ?? new List<BlogImage>();

                blog.Edit(dto.Title, dto.Description, images);
            }
            else if (blog.State == BlogState.Published)
            {
                blog.EditDescription(dto.Description);
            }
            else if (blog.State == BlogState.Archived)
            {
                throw new InvalidOperationException("Archived blogs cannot be edited.");
            }

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

        public async Task ArchiveAsync(long id, long userId)
        {
            var blog = await _repository.GetByIdAsync(id);
            if (blog == null)
                throw new KeyNotFoundException();

            if (blog.AuthorId != userId)
                throw new UnauthorizedAccessException();

            blog.Archive();
            await _repository.UpdateAsync(blog);
        }

        public async Task<IEnumerable<BlogPostDto>> GetVisibleBlogsAsync(long? currentUserId, int? filterStatus = null)
        {
            var allBlogs = await _repository.GetAllAsync();

            var visibleBlogs = allBlogs.Where(b =>
                b.State != BlogState.Draft || (currentUserId.HasValue && b.AuthorId == currentUserId.Value)
            );

            if (filterStatus.HasValue)
            {
                visibleBlogs = visibleBlogs.Where(b => (int)b.State == filterStatus.Value);
            }

            var dtos = _mapper.Map<IEnumerable<BlogPostDto>>(visibleBlogs);

            foreach (var dto in dtos)
            {
                var blog = visibleBlogs.First(b => b.Id == dto.Id);
                dto.State = (int)blog.State;
            }

            return dtos;
        }

        public async Task<VoteResultDto> AddVoteAsync(long blogPostId, int voteValue, long userId)
        {
            if (voteValue != 1 && voteValue != -1)
                throw new ArgumentException("Vote value must be 1 or -1");

            var blog = await _repository.GetByIdAsync(blogPostId);
            if (blog == null)
                throw new KeyNotFoundException("Blog post not found");

            var vote = voteValue == 1 ? VoteValue.Upvote : VoteValue.Downvote;

            if (blog.State == BlogState.Closed)
                throw new InvalidOperationException("Cannot vote on a closed blog.");


            var comments =  _commentRepository.GetByBlogPost(blogPostId); // treba da dodaš ICommentRepository u konstruktor
            

            blog.AddVote(userId, vote);
            blog.UpdateStatus(comments.Count);

            await _repository.UpdateAsync(blog);

            return new VoteResultDto
            {
                Score = blog.GetScore(),
                UpvoteCount = blog.GetUpvoteCount(),
                DownvoteCount = blog.GetDownvoteCount(),
                UserVote = blog.GetUserVote(userId)?.Value
            };
        }

        public async Task<VoteResultDto> RemoveVoteAsync(long blogPostId, long userId)
        {
            var blog = await _repository.GetByIdAsync(blogPostId);
            if (blog == null)
                throw new KeyNotFoundException("Blog post not found");

            blog.RemoveVote(userId);

            await _repository.UpdateAsync(blog);

            return new VoteResultDto
            {
                Score = blog.GetScore(),
                UpvoteCount = blog.GetUpvoteCount(),
                DownvoteCount = blog.GetDownvoteCount(),
                UserVote = null
            };
        }

        // Helper metoda za mapiranje sa Vote informacijama
        private BlogPostDto MapToDtoWithVotes(BlogPost blog, long? currentUserId)
        {
            var dto = _mapper.Map<BlogPostDto>(blog);
            dto.State = (int)blog.State;

            dto.Score = blog.GetScore();
            dto.UpvoteCount = blog.GetUpvoteCount();
            dto.DownvoteCount = blog.GetDownvoteCount();
            dto.UserVote = currentUserId.HasValue ? blog.GetUserVote(currentUserId.Value)?.Value : null;

            return dto;
        }
    }
}
