using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces
{
    public interface IEncounterPresenceRepository
    {
        void Upsert(long encounterId, long userId, double lat, double lon);
        void Remove(long encounterId, long userId);
        void RemoveOlderThan(long encounterId, DateTime cutoffUtc);
        List<long> GetActiveUserIds(long encounterId, DateTime cutoffUtc);
        int CountActive(long encounterId, DateTime cutoffUtc);
    }
}
