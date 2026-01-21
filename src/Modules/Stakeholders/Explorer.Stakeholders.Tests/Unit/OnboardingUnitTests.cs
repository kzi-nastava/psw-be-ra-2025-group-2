using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Unit
{
    public class OnboardingUnitTests
    {
        [Fact]
        public void Constructor_creates_valid_onboarding_slide()
        {
            // Arrange & Act
            var slide = new OnboardingSlide("Title", "Body", 1, 0);

            // Assert
            slide.Title.ShouldBe("Title");
            slide.BodyText.ShouldBe("Body");
            slide.Ordinal.ShouldBe(1);
            slide.Role.ShouldBe(0);
        }

        [Fact]
        public void Constructor_fails_with_empty_title()
        {
            // Act & Assert
            var exception = Should.Throw<ArgumentException>(() =>
                new OnboardingSlide("", "Body", 1, 0));

            exception.Message.ShouldContain("Title cannot be empty");
        }
    }
}
