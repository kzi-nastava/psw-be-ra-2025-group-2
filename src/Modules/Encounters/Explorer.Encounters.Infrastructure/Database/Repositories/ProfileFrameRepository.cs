using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class ProfileFrameRepository : IProfileFrameRepository
    {
        private readonly EncountersContext _context;

        public ProfileFrameRepository(EncountersContext context)
        {
            _context = context;
        }

        public ProfileFrame? GetByLevelRequirement(int levelRequirement)
        {
            return _context.ProfileFrames
                .AsNoTracking()
                .FirstOrDefault(x => x.LevelRequirement == levelRequirement);
        }

        public IEnumerable<ProfileFrame> GetAll()
        {
            return _context.ProfileFrames
                .AsNoTracking()
                .OrderBy(x => x.LevelRequirement)
                .ToList();
        }

        public void Add(ProfileFrame frame)
        {
            _context.ProfileFrames.Add(frame);
            _context.SaveChanges();
        }
    }
}