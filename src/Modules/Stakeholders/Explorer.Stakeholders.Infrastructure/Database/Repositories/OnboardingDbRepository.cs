using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class OnboardingDbRepository : IOnboardingRepository
    {
        private readonly StakeholdersContext _dbContext;

        public OnboardingDbRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<OnboardingSlide> GetByRole(int role)
        {
            return _dbContext.OnboardingSlides
                             .Where(s => s.Role == role)
                             .ToList();
        }

        public OnboardingProgress GetProgressByUserId(long userId)
        {
            return _dbContext.OnboardingProgresses
                             .FirstOrDefault(p => p.UserId == userId);
        }

        public OnboardingProgress CreateProgress(OnboardingProgress progress)
        {
            _dbContext.OnboardingProgresses.Add(progress);
            _dbContext.SaveChanges();
            return progress;
        }

        public OnboardingProgress UpdateProgress(OnboardingProgress progress)
        {
            _dbContext.OnboardingProgresses.Update(progress);
            _dbContext.SaveChanges();
            return progress;
        }
    }
}
