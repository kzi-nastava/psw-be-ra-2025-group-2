using Explorer.Encounters.Core.Domain;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Tests.Unit
{
    public class ExperiencePointsUnitTests
    {
        [Fact]
        public void Creates_with_positive_value()
        {
            var xp = new ExperiencePoints(100);

            xp.Value.ShouldBe(100);
        }

        [Fact]
        public void Zero_returns_zero_value()
        {
            var xp = ExperiencePoints.Zero;

            xp.Value.ShouldBe(0);
        }

        [Fact]
        public void Creation_fails_for_negative_value()
        {
            Should.Throw<ArgumentOutOfRangeException>(() =>
                new ExperiencePoints(-1));
        }

        [Fact]
        public void Adds_experience_points()
        {
            var a = new ExperiencePoints(50);
            var b = new ExperiencePoints(30);

            var result = a + b;

            result.Value.ShouldBe(80);
        }

        [Fact]
        public void Subtracts_experience_points()
        {
            var a = new ExperiencePoints(100);
            var b = new ExperiencePoints(40);

            var result = a - b;

            result.Value.ShouldBe(60);
        }

        [Fact]
        public void Subtraction_fails_when_result_would_be_negative()
        {
            var a = new ExperiencePoints(30);
            var b = new ExperiencePoints(50);

            Should.Throw<ArgumentOutOfRangeException>(() => a - b);
        }

        [Fact]
        public void Multiplies_with_rounding_away_from_zero()
        {
            var xp = new ExperiencePoints(10);

            var result = xp * 1.25;

            result.Value.ShouldBe(13);
        }

        [Fact]
        public void Multiplier_cannot_be_negative()
        {
            var xp = new ExperiencePoints(10);

            Should.Throw<ArgumentOutOfRangeException>(() => xp * -1);
        }

        [Fact]
        public void Supports_reverse_multiplication()
        {
            var xp = new ExperiencePoints(20);

            var result = 2.0 * xp;

            result.Value.ShouldBe(40);
        }

        [Fact]
        public void Supports_comparison_operators()
        {
            var low = new ExperiencePoints(10);
            var high = new ExperiencePoints(50);

            (low < high).ShouldBeTrue();
            (high > low).ShouldBeTrue();
            (low <= high).ShouldBeTrue();
            (high >= low).ShouldBeTrue();
        }

        [Fact]
        public void Equality_is_value_based()
        {
            var a = new ExperiencePoints(100);
            var b = new ExperiencePoints(100);
            var c = new ExperiencePoints(200);

            (a == b).ShouldBeTrue();
            (a != c).ShouldBeTrue();
            a.Equals(b).ShouldBeTrue();
        }

        [Fact]
        public void Supports_implicit_conversion_to_int()
        {
            var xp = new ExperiencePoints(75);

            int value = xp;

            value.ShouldBe(75);
        }

        [Fact]
        public void Supports_explicit_conversion_from_int()
        {
            var xp = (ExperiencePoints)120;

            xp.Value.ShouldBe(120);
        }
    }

}
