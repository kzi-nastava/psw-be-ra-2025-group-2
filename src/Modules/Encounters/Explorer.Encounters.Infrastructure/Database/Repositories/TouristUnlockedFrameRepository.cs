using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class TouristUnlockedFrameRepository : ITouristUnlockedFrameRepository
    {
        private readonly EncountersContext _context;

        public TouristUnlockedFrameRepository(EncountersContext context)
        {
            _context = context;
        }

        public bool Exists(long userId, long frameId)
        {
            return _context.TouristUnlockedFrames
                .AsNoTracking()
                .Any(x => x.UserId == userId && x.FrameId == frameId);
        }

        public IEnumerable<TouristUnlockedFrame> GetByUserId(long userId)
        {
            return _context.TouristUnlockedFrames
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.UnlockedAt)
                .ToList();
        }

        public void Add(TouristUnlockedFrame unlocked)
        {
            _context.TouristUnlockedFrames.Add(unlocked);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg && pg.SqlState == "23505")
            {
                
            }
        }

    }
}