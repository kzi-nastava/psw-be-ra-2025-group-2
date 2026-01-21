using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IOnboardingService
    {
        List<OnboardingSlideDto> GetSlidesForRole(int role);

        OnboardingProgressDto GetProgress(long userId);
        OnboardingProgressDto CompleteOnboarding(long userId);
    }
}
