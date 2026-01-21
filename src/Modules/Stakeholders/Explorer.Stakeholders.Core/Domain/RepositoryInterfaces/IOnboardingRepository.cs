using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IOnboardingRepository
    {
        List<OnboardingSlide> GetByRole(int role);
        OnboardingProgress GetProgressByUserId(long userId);
        OnboardingProgress CreateProgress(OnboardingProgress progress);
        OnboardingProgress UpdateProgress(OnboardingProgress progress);
    }
}
