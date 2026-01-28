using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IClubChatService
    {
        ClubMessageDto Send(long clubId, long senderId, string content);
        List<ClubMessageDto> GetMessages(long clubId, long userId);
        ClubMessageDto Edit(long clubId, long userId, long messageId, string content);
        void Delete(long clubId, long userId, long messageId);

    }
}
