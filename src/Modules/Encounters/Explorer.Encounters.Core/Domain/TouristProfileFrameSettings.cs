using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Encounters.Core.Domain
{
    public class TouristProfileFrameSettings : Entity
    {
        public long UserId { get; private set; }
        public bool ShowFramesEnabled { get; private set; }
        public long? SelectedFrameId { get; private set; } // null = "bez rama"

        // EF
        protected TouristProfileFrameSettings() { }

        public TouristProfileFrameSettings(long userId)
        {
            if (userId <= 0) throw new ArgumentException("UserId must be > 0.");
            UserId = userId;
            ShowFramesEnabled = true; // default: prikaz uključen
            SelectedFrameId = null;
        }

        public void SetShowFrames(bool enabled) => ShowFramesEnabled = enabled;

        public void SetSelectedFrame(long? frameId) => SelectedFrameId = frameId;
    }
}