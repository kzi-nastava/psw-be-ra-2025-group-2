using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.Stakeholders.API.Dtos.Messages;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repository;
        private readonly IMapper _mapper;

        public MessageService(IMessageRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public MessageDto Send(long senderId, SendMessageDto dto)
        {
            var message = new Message(senderId, dto.ReceiverId, dto.Content);
            return _mapper.Map<MessageDto>(_repository.Create(message));
        }

        public List<MessageDto> GetAllForUser(long userId) 
        {
            return _repository.GetForUser(userId).Where(m => !m.IsDeleted).Select(_mapper.Map<MessageDto>).ToList();
        }

        public MessageDto Edit(long userId, long messageId, string content)
        {
            var message = _repository.GetById(messageId) ?? throw new ArgumentException("Message not found");

            message.Edit(userId, content);
            return _mapper.Map<MessageDto>(_repository.Update(message));
        }

        public void Delete(long userId, long messageId)
        {
            var message = _repository.GetById(messageId) ?? throw new ArgumentException("Message not found");

            message.Delete(userId);
            _repository.Update(message);
        }
    }
}
