using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class OnboardingProgress : Entity
    {
        public long UserId { get; private set; }
        public bool HasCompletedOnboarding { get; private set; }

        public OnboardingProgress(long userId, bool hasCompletedOnboarding)
        {
            UserId = userId;
            HasCompletedOnboarding = hasCompletedOnboarding;
        }

        public void MarkAsCompleted()
        {
            HasCompletedOnboarding = true;
        }
    }
}
