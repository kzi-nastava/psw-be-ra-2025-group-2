using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IMessageRepository
    {
        Message Create(Message message);
        Message? GetById(long id);
        List<Message> GetForUser(long userId);
        Message Update(Message message);

        List<Message> GetConversation(long user1, long user2);
    }
}
