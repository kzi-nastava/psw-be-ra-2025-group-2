using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Encounters.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class EncounterPresenceRepository : IEncounterPresenceRepository
    {
        private readonly EncountersContext _context;

        public EncounterPresenceRepository(EncountersContext context)
        {
            _context = context;
        }

        public void Upsert(long encounterId, long userId, double lat, double lon)
        {
            var existing = _context.EncounterPresences
                .FirstOrDefault(x => x.EncounterId == encounterId && x.UserId == userId);

            if (existing == null)
            {
                _context.EncounterPresences.Add(new EncounterPresence(encounterId, userId, lat, lon));
            }
            else
            {
                existing.Update(lat, lon);
            }

            _context.SaveChanges();
        }

        public void Remove(long encounterId, long userId)
        {
            var existing = _context.EncounterPresences
                .FirstOrDefault(x => x.EncounterId == encounterId && x.UserId == userId);

            if (existing == null) return;

            _context.EncounterPresences.Remove(existing);
            _context.SaveChanges();
        }

        public void RemoveOlderThan(long encounterId, DateTime cutoffUtc)
        {
            var stale = _context.EncounterPresences
                .Where(x => x.EncounterId == encounterId && x.LastSeenAt < cutoffUtc)
                .ToList();

            if (stale.Count == 0) return;

            _context.EncounterPresences.RemoveRange(stale);
            _context.SaveChanges();
        }

        public List<long> GetActiveUserIds(long encounterId, DateTime cutoffUtc)
        {
            return _context.EncounterPresences
                .Where(x => x.EncounterId == encounterId && x.LastSeenAt >= cutoffUtc)
                .Select(x => x.UserId)
                .ToList();
        }

        public int CountActive(long encounterId, DateTime cutoffUtc)
        {
            return _context.EncounterPresences
                .Count(x => x.EncounterId == encounterId && x.LastSeenAt >= cutoffUtc);
        }
    }
}
