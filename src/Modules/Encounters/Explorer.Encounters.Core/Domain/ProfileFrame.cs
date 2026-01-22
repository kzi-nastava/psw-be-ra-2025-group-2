using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Encounters.Core.Domain
{
    public class ProfileFrame : Entity
    {
        public string Name { get; private set; }
        public int LevelRequirement { get; private set; }
        public string AssetKey { get; private set; } 
        
        protected ProfileFrame() { }

        public ProfileFrame(string name, int levelRequirement, string assetKey)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
            if (levelRequirement <= 0) throw new ArgumentException("LevelRequirement must be > 0.");
            if (string.IsNullOrWhiteSpace(assetKey)) throw new ArgumentException("AssetKey is required.");

            Name = name;
            LevelRequirement = levelRequirement;
            AssetKey = assetKey;
        }
    }
}