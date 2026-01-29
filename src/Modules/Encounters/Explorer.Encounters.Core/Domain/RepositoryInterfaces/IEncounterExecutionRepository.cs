using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces
{
    public interface IEncounterExecutionRepository
    {
        EncounterExecution Add(EncounterExecution execution);
        bool IsCompleted(long userId, long encounterId);
        EncounterExecution? Get(long userId, long encounterId);
        EncounterExecution Update(EncounterExecution execution);

        List<EncounterExecution> GetActiveByEncounter(long encounterId);
        List<EncounterExecution> GetByEncounterIds(IEnumerable<long> encounterIds);
        void Delete(long id);
    }
}
