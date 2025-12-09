using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Internal;
using AutoMapper;
using System.Collections.Generic;
using Explorer.Stakeholders.API.Internal;

namespace Explorer.Blog.Core.UseCases
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUsernameProvider _userNameProvider;
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IMapper _mapper;

        public CommentService(ICommentRepository commentRepository, IBlogPostRepository blogPostRepository, IUsernameProvider userNameProvider, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _blogPostRepository = blogPostRepository;
            _userNameProvider = userNameProvider;
            _mapper = mapper;
        }

        public CommentDto Create(CommentDto commentDto)
        {
            // Pokupi username
            var username = _userNameProvider.GetNameById(commentDto.UserId);
            commentDto.UserName = username;

            var blog = _blogPostRepository.GetByIdAsync(commentDto.BlogPostId).GetAwaiter().GetResult();
            if (blog == null)
                throw new KeyNotFoundException("Blog not found.");

            if (blog.State == BlogState.Closed)
                throw new InvalidOperationException("Cannot comment on a closed blog.");

            var entity = _mapper.Map<Comment>(commentDto);
            var created = _commentRepository.Create(entity);

            var comments = _commentRepository.GetByBlogPost(blog.Id);

            blog.UpdateStatus(comments.Count);
            _blogPostRepository.UpdateAsync(blog).GetAwaiter().GetResult();
            return _mapper.Map<CommentDto>(created);
        }

        public CommentDto Update(CommentDto commentDto, long commentId)
        {
            var entity = _commentRepository.Get(commentId);
            if (entity == null)
                throw new KeyNotFoundException($"Comment with id {commentId} not found.");

            entity.UpdateText(commentDto.Text);

            var updated = _commentRepository.Update(entity);
            return _mapper.Map<CommentDto>(updated);
        }

        public void Delete(long id)
        {
            _commentRepository.Delete(id);
        }

        public List<CommentDto> GetAll()
        {
            var comments = _commentRepository.GetAll();
            var dtos = _mapper.Map<List<CommentDto>>(comments);

            // Efikasnije - pokupi sve usernames odjednom
            var userIds = dtos.Select(d => d.UserId).Distinct();
            var usernames = _userNameProvider.GetNamesByIds(userIds);

            foreach (var dto in dtos)
            {
                dto.UserName = usernames.ContainsKey(dto.UserId)
                    ? usernames[dto.UserId]
                    : "Unknown";
            }

            return dtos;
        }

        public CommentDto Get(long id)
        {
            var comment = _commentRepository.Get(id);
            if (comment == null)
                return null;

            var dto = _mapper.Map<CommentDto>(comment);

            // Pokupi username
            dto.UserName = _userNameProvider.GetNameById(comment.UserId);

            return dto;
        }
        public List<CommentDto> GetByBlogPost(long blogPostId)
        {
            var comments = _commentRepository.GetByBlogPost(blogPostId);
            var dtos = _mapper.Map<List<CommentDto>>(comments);

            // Efikasnije - pokupi sve usernames odjednom
            var userIds = dtos.Select(d => d.UserId).Distinct();
            var usernames = _userNameProvider.GetNamesByIds(userIds);

            foreach (var dto in dtos)
            {
                dto.UserName = usernames.ContainsKey(dto.UserId)
                    ? usernames[dto.UserId]
                    : "Unknown";
            }

            return dtos;
        }
    }
}