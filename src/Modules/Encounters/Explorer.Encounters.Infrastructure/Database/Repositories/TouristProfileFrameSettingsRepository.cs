using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class TouristProfileFrameSettingsRepository : ITouristProfileFrameSettingsRepository
    {
        private readonly EncountersContext _context;

        public TouristProfileFrameSettingsRepository(EncountersContext context)
        {
            _context = context;
        }

        public TouristProfileFrameSettings? GetByUserId(long userId)
        {
            return _context.TouristProfileFrameSettings
                .FirstOrDefault(x => x.UserId == userId);
        }

        public TouristProfileFrameSettings Create(TouristProfileFrameSettings settings)
        {
            _context.TouristProfileFrameSettings.Add(settings);
            _context.SaveChanges();
            return settings;
        }

        public void Update(TouristProfileFrameSettings settings)
        {
            _context.TouristProfileFrameSettings.Update(settings);
            _context.SaveChanges();
        }
    }
}