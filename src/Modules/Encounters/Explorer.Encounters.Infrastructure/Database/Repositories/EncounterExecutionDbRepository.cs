using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class EncounterExecutionRepository : IEncounterExecutionRepository
    {
        private readonly EncountersContext _context;

        public EncounterExecutionRepository(EncountersContext context)
        {
            _context = context;
        }

        public EncounterExecution Add(EncounterExecution execution)
        {
            _context.EncounterExecutions.Add(execution);
            _context.SaveChanges();
            return execution;
        }

        public bool IsCompleted(long userId, long encounterId)
        {
            return _context.EncounterExecutions
                .Any(e => e.UserId == userId && e.EncounterId == encounterId && e.IsCompleted);
        }
    }
}
