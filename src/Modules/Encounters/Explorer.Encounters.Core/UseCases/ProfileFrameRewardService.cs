using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using System;

namespace Explorer.Encounters.Core.UseCases
{
    public class ProfileFrameRewardService
    {
        private readonly IProfileFrameRepository _frameRepo;
        private readonly ITouristUnlockedFrameRepository _unlockedRepo;

        public ProfileFrameRewardService(
            IProfileFrameRepository frameRepo,
            ITouristUnlockedFrameRepository unlockedRepo)
        {
            _frameRepo = frameRepo;
            _unlockedRepo = unlockedRepo;
        }

        public void AwardFramesIfNeeded(long userId, int oldLevel, int newLevel)
        {
            if (userId <= 0) throw new ArgumentException("userId must be > 0");
            if (newLevel <= oldLevel) return;

            // Otključava pragove: 5,10,15... u (oldLevel, newLevel]
            for (int lvl = oldLevel + 1; lvl <= newLevel; lvl++)
            {
                if (lvl % 5 != 0) continue;

                var frame = _frameRepo.GetByLevelRequirement(lvl);
                if (frame == null) continue; 

                
                if (_unlockedRepo.Exists(userId, frame.Id)) continue;

                _unlockedRepo.Add(new TouristUnlockedFrame(
                    userId,
                    frame.Id,
                    DateTime.UtcNow
                ));
            }
        }
    }
}