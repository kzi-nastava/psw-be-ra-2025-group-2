using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class TouristProgressRepository : ITouristProgressRepository
    {
        private readonly EncountersContext _context;

        public TouristProgressRepository(EncountersContext context)
        {
            _context = context;
        }

        public TouristProgress GetByUserId(long userId)
        {
            return _context.TouristProgresses
                .FirstOrDefault(tp => tp.UserId == userId);
        }

        public TouristProgress Create(TouristProgress progress)
        {
            _context.TouristProgresses.Add(progress);
            _context.SaveChanges();
            return progress;
        }

        public TouristProgress Update(TouristProgress progress)
        {
            _context.TouristProgresses.Update(progress);
            _context.SaveChanges();
            return progress;
        }

        public List<TouristProgress> GetByUserIds(IEnumerable<long> userIds)
        {
            var ids = userIds.Distinct().ToList();
            return _context.TouristProgresses.Where(tp => ids.Contains(tp.UserId)).ToList();
        }

    }
}
