using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ClubChatService : IClubChatService
    {
        private readonly IClubRepository _clubRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public ClubChatService(
        IClubRepository clubRepo,
        IChatRepository chatRepo,
        IMessageRepository messageRepo,
        IMapper mapper)
        {
            _clubRepository = clubRepo;
            _chatRepository = chatRepo;
            _messageRepository = messageRepo;
            _mapper = mapper;
        }

        public ClubMessageDto Send(long clubId, long senderId, string content)
        {
            var club = _clubRepository.Get(clubId);

            bool isOwner = club.OwnerId == senderId;
            bool isMember = club.Members.Any(m => m.TouristId == senderId);

            if (!isOwner && !isMember)
                throw new UnauthorizedAccessException("Not a club member");

            var chat = _chatRepository.GetByClubId(clubId)
                       ?? _chatRepository.Create(Chat.CreateClub(clubId));

            var message = Message.CreateChat(chat.Id, senderId, content);
            return _mapper.Map<ClubMessageDto>(_messageRepository.Create(message));
        }

        public List<ClubMessageDto> GetMessages(long clubId, long userId)
        {
            var club = _clubRepository.Get(clubId);

            bool isOwner = club.OwnerId == userId;
            bool isMember = club.Members.Any(m => m.TouristId == userId);

            if (!isOwner && !isMember)
                throw new UnauthorizedAccessException("Not a club member");

            var chat = _chatRepository.GetByClubId(clubId);
            if (chat == null) return new();

            return _messageRepository
                .GetByChat(chat.Id)
                .Select(_mapper.Map<ClubMessageDto>)
                .ToList();
        }

    }
}
