using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain
{
    public class ExperiencePoints : ValueObject
    {
        public int Value { get; init; }

        public static ExperiencePoints Zero => new ExperiencePoints(0);

        private ExperiencePoints() { }

        public ExperiencePoints(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Points must be positive.");

            Value = value;
        }

        public static ExperiencePoints operator+(ExperiencePoints a, ExperiencePoints b)
        {
            return new ExperiencePoints(a.Value + b.Value);
        }

        public static ExperiencePoints operator-(ExperiencePoints a, ExperiencePoints b)
        {
            if (a.Value < b.Value)
                throw new ArgumentOutOfRangeException(nameof(b), $"Invalid arguments: cannot subtract {b.Value} XP from {a.Value} XP.");
            return new ExperiencePoints(a.Value - b.Value);
        }

        public static ExperiencePoints operator*(ExperiencePoints a, double multiplier)
        {
            if (multiplier < 0)
                throw new ArgumentOutOfRangeException(nameof(multiplier), "Multiplier must be greater than 0.");

            return new ExperiencePoints((int)Math.Round(a.Value * multiplier, 0, MidpointRounding.AwayFromZero));
        }

        public static ExperiencePoints operator *(double multiplier, ExperiencePoints a) => a * multiplier;

        public static bool operator <(ExperiencePoints a, ExperiencePoints b) => a.Value < b.Value;
        public static bool operator >(ExperiencePoints a, ExperiencePoints b) => a.Value > b.Value;
        public static bool operator <=(ExperiencePoints a, ExperiencePoints b) => a.Value <= b.Value;
        public static bool operator >=(ExperiencePoints a, ExperiencePoints b) => a.Value >= b.Value;
        public static bool operator ==(ExperiencePoints a, ExperiencePoints b) => a is null ? b is null : a.Equals(b);
        public static bool operator !=(ExperiencePoints a, ExperiencePoints b) => !(a == b);

        public static implicit operator int(ExperiencePoints x) => x.Value;
        public static explicit operator ExperiencePoints(int value) => new ExperiencePoints(value);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
