using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos.Messages;

namespace Explorer.Stakeholders.API.Public
{
    public interface IMessageService
    {
        MessageDto Send(long senderId, SendMessageDto dto);
        List<MessageDto> GetAllForUser(long userId);
        MessageDto Edit(long userId, long messageId, string content);
        void Delete(long userId, long messageId);
        List<MessageDto> GetConversation(long me, long other);
    }
}
