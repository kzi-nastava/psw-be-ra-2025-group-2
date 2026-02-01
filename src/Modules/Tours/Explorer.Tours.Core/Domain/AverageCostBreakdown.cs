using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;

namespace Explorer.Tours.Core.Domain
{
    public class AverageCostBreakdown : ValueObject
    {
        public decimal Tickets { get; }
        public decimal Transport { get; }
        public decimal FoodAndDrink { get; }
        public decimal Other { get; }

        private AverageCostBreakdown() { } // EF

        private AverageCostBreakdown(decimal tickets, decimal transport, decimal foodAndDrink, decimal other)
        {
            ValidateNonNegative(tickets, nameof(tickets));
            ValidateNonNegative(transport, nameof(transport));
            ValidateNonNegative(foodAndDrink, nameof(foodAndDrink));
            ValidateNonNegative(other, nameof(other));

            Tickets = Round(tickets);
            Transport = Round(transport);
            FoodAndDrink = Round(foodAndDrink);
            Other = Round(other);
        }

        public static AverageCostBreakdown Create(decimal tickets, decimal transport, decimal foodAndDrink, decimal other)
            => new AverageCostBreakdown(tickets, transport, foodAndDrink, other);

        public decimal Total()
            => Tickets + Transport + FoodAndDrink + Other;

        private static void ValidateNonNegative(decimal value, string name)
        {
            if (value < 0)
                throw new ArgumentException($"{name} cannot be negative.");
        }

        private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Tickets;
            yield return Transport;
            yield return FoodAndDrink;
            yield return Other;
        }
    }
}
