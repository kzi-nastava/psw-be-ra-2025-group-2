using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class OnboardingSlide : AggregateRoot
    {
        public string Title { get; private set; }
        public string BodyText { get; private set; }
        public int Ordinal { get; private set; }
        public int Role { get; private set; } // Mapiraćemo na enum ili int

        public OnboardingSlide(string title, string bodyText, int ordinal, int role)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title cannot be empty");
            Title = title;
            BodyText = bodyText;
            Ordinal = ordinal;
            Role = role;
        }
    }
}
