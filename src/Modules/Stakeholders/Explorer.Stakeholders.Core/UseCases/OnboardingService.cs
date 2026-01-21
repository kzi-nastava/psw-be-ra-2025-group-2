using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class OnboardingService : IOnboardingService
    {
        private readonly IOnboardingRepository _repository;
        private readonly IMapper _mapper;

        public OnboardingService(IOnboardingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public List<OnboardingSlideDto> GetSlidesForRole(int role)
        {
            var slides = _repository.GetByRole(role).OrderBy(s => s.Ordinal).ToList();
            return _mapper.Map<List<OnboardingSlideDto>>(slides);
        }

        public OnboardingProgressDto GetProgress(long userId)
        {
            var progress = _repository.GetProgressByUserId(userId);
            if (progress == null)
            {
                return new OnboardingProgressDto { UserId = userId, HasCompletedOnboarding = false };
            }
            return _mapper.Map<OnboardingProgressDto>(progress);
        }

        public OnboardingProgressDto CompleteOnboarding(long userId)
        {
            var progress = _repository.GetProgressByUserId(userId);

            if (progress == null)
            {
                progress = new OnboardingProgress(userId, true);
                _repository.CreateProgress(progress);
            }
            else
            {
                progress.MarkAsCompleted();
                _repository.UpdateProgress(progress);
            }

            return _mapper.Map<OnboardingProgressDto>(progress);
        }


    }
}
