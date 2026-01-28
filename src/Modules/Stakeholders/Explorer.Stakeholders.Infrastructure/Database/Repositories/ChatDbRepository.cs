using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class ChatDbRepository : IChatRepository
    {
        private readonly StakeholdersContext _dbContext;

        public ChatDbRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Chat Create(Chat chat)
        {
            _dbContext.Chats.Add(chat);
            _dbContext.SaveChanges();
            return chat;
        }

        public Chat? Get(long id)
        {
            return _dbContext.Chats.Find(id);
        }
        public Chat? GetByClubId(long clubId)
        {
            return _dbContext.Chats.FirstOrDefault(c=>c.ChatType == ChatType.Club && c.ClubId == clubId);
        }
    }
}
