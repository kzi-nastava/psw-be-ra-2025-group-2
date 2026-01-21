using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Stakeholders
{
    [Route("api/onboarding")]
    [ApiController]
    public class OnboardingController : ControllerBase
    {
        private readonly IOnboardingService _onboardingService;

        public OnboardingController(IOnboardingService onboardingService)
        {
            _onboardingService = onboardingService;
        }

        [HttpGet("{role:int}")]
        public ActionResult<List<OnboardingSlideDto>> GetSlides(int role)
        {
            // role: 0 = Tourist, 1 = Author
            var result = _onboardingService.GetSlidesForRole(role);
            return Ok(result);
        }

        [HttpGet("progress/{userId:long}")]
        public ActionResult<OnboardingProgressDto> GetProgress(long userId)
        {
            var result = _onboardingService.GetProgress(userId);
            return Ok(result);
        }

        [HttpPost("complete/{userId:long}")]
        public ActionResult<OnboardingProgressDto> Complete(long userId)
        {
            var result = _onboardingService.CompleteOnboarding(userId);
            return Ok(result);
        }
    }
}
