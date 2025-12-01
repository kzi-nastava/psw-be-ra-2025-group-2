using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Internal;
using System;
public class CommentService : ICommentService
{

    private readonly ICommentRepository _commentRepository; 
    private readonly IUsernameProvider _usernameProvider;
    private readonly IMapper _mapper;


    public CommentService(ICommentRepository commentRepository, IMapper mapper, IUsernameProvider usernameProvider)
    {
        _commentRepository = commentRepository;
        _mapper = mapper;
        _usernameProvider = usernameProvider;
    }

    public CommentDto Create(long userId, string text)
    {
   
            var username = _usernameProvider.GetNameById(userId);
            if (string.IsNullOrEmpty(username))
                throw new Exception("User not found.");

            var comment = new Comment(userId, text);
            _commentRepository.Create(comment);

            return new CommentDto
            {
                Username = username,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                LastModifiedAt = comment.LastModifiedAt
            };
        
    }

    public void Edit(long userId, DateTime createdAt, string newText)
    {
        var username = _usernameProvider.GetNameById(userId); if (string.IsNullOrEmpty(username)) throw new Exception("User not found.");
        var comment = _commentRepository.Get(username, createdAt);
        if (comment == null)
            throw new Exception("Comment not found.");
        if (!comment.CanEditOrDelete())
            throw new Exception("Edit window expired.");

        comment.Edit(newText);
        _commentRepository.Update(comment);
    }

    public void Delete(long userId, DateTime createdAt)
    {
        var username = _usernameProvider.GetNameById(userId); if (string.IsNullOrEmpty(username)) throw new Exception("User not found.");
        var comment = _commentRepository.Get(username, createdAt);
        if (comment == null)
            throw new Exception("Comment not found.");
        if (!comment.CanEditOrDelete())
            throw new Exception("Delete window expired.");

        _commentRepository.Delete(username, createdAt);
    }



}
