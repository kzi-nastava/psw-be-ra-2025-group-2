using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain.Help;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class FaqRepository : IFaqRepository
    {
        private readonly StakeholdersContext _context;

        public FaqRepository(StakeholdersContext context) 
        {
            _context = context;
        }

        public List<FaqItem> GetActive() => _context.FaqItems.Where(f => f.IsActive).ToList();

        public FaqItem Get(long id) => _context.FaqItems.Find(id);

        public FaqItem Create(FaqItem item) 
        {
            _context.FaqItems.Add(item);
            _context.SaveChanges();
            return item;
        }

        public FaqItem Update(FaqItem item) 
        {
            _context.FaqItems.Update(item);
            _context.SaveChanges() ;
            return item;
        }
    }
}
