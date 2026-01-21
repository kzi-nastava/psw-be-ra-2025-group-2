using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class OnboardingSlideDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string BodyText { get; set; }
        public int Ordinal { get; set; } // Redoslijed slajda
        public OnBoardingRole Role { get; set; } // 0 turista, 1 autor
    }

    public enum OnBoardingRole { Tourist, Author }
}
