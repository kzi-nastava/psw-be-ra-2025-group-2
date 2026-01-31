using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;

namespace Explorer.Tours.Core.Domain
{
    public class AverageCost : ValueObject
    {
        public decimal TotalPerPerson { get; }
        public string Currency { get; }
        public AverageCostBreakdown Breakdown { get; }
        public string Disclaimer { get; }

        private AverageCost() { } // EF

        private AverageCost(decimal totalPerPerson, string currency, AverageCostBreakdown breakdown, string disclaimer)
        {
            if (totalPerPerson < 0)
                throw new ArgumentException("TotalPerPerson cannot be negative.");

            Breakdown = breakdown ?? throw new ArgumentNullException(nameof(breakdown));

            Currency = NormalizeCurrency(currency);

            if (string.IsNullOrWhiteSpace(disclaimer))
                throw new ArgumentException("Disclaimer is required.");

            TotalPerPerson = Round(totalPerPerson);
            Disclaimer = disclaimer.Trim();
        }

        public static AverageCost Create(string currency, AverageCostBreakdown breakdown, string disclaimer)
            => new AverageCost(breakdown.Total(), currency, breakdown, disclaimer);

        public static AverageCost Create(decimal totalPerPerson, string currency, AverageCostBreakdown breakdown, string disclaimer)
            => new AverageCost(totalPerPerson, currency, breakdown, disclaimer);

        private static string NormalizeCurrency(string currency)
        {
            var c = (currency ?? "").Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(c)) return "RSD";
            if (c is "RSD" or "EUR") return c;

            
            throw new ArgumentException("Currency must be RSD or EUR.");
        }

        private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TotalPerPerson;
            yield return Currency;
            yield return Breakdown;
            yield return Disclaimer;
        }
    }
}
