using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class OnboardingProgressDto
    {
        public long UserId { get; set; }
        public bool HasCompletedOnboarding {  get; set; }
    }
}
