using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class MessageDbRepository : IMessageRepository
    {
        private readonly StakeholdersContext _dbContext;

        public MessageDbRepository(StakeholdersContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public Message Create(Message message) 
        {
            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();
            return message;
        }

        public Message? GetById(long id)
        {
            return _dbContext.Messages.Find(id);
        }

        public List<Message> GetForUser(long userId) 
        {
            return _dbContext.Messages
                             .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                             .OrderBy(m => m.CreatedAt)
                             .ToList();
        }

        public Message Update(Message message)
        {
            _dbContext.Messages.Update(message);
            _dbContext.SaveChanges();
            return message;
        }
    }
}
