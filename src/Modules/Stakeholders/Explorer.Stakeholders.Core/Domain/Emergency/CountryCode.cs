using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.Emergency
{
    public class CountryCode : ValueObject
    {
        public string Value { get; private set; }

        private CountryCode() { } // EF

        public CountryCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Country code cannot be empty.");

            var normalized = value.Trim().ToUpperInvariant();

            // ISO2/ISO3
            if (normalized.Length < 2 || normalized.Length > 3)
                throw new ArgumentException("Country code must have 2 or 3 characters.");

            Value = normalized;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
