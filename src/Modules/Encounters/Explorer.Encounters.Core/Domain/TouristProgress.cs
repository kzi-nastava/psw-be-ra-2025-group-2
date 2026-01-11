using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain
{
    public class TouristProgress : Entity
    {
        public long UserId { get; private set; }
        public int TotalXp { get; private set; }
        public int Level { get; private set; }
        public bool CanCreateChallenges { get; private set; }

        protected TouristProgress() { }

        public TouristProgress(long userId)
        {
            UserId = userId;
            TotalXp = 0;
            Level = 1;
            CanCreateChallenges = false;
        }

        public bool AddXp(int xp)
        {
            TotalXp += xp;
            int prevLevel = Level;
            Level = CalculateLevel();

            // kada predje nivo 10, moze kreirati izazove
            if (Level >= 10)
                CanCreateChallenges = true;

            return Level > prevLevel;
        }

        private int CalculateLevel()
        {
            return TotalXp / 100 + 1; // svakih 100 XP dodaje nivo
        }
    }
}