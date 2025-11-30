using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.TourSession
{
    public class KeyPointVisit : Entity
    {
        public int KeyPointOrdinal { get; init; }
        public DateTime ArrivalTimestamp { get; init; }

        public KeyPointVisit(int keyPointOrdinal, DateTime arrivalTimestamp)
        {
            KeyPointOrdinal = keyPointOrdinal;
            ArrivalTimestamp = EnsureUtc(arrivalTimestamp);
            Validate();
        }

        private void Validate()
        {
            if (KeyPointOrdinal < 1)
                throw new EntityValidationException("Cannot arrive at a key point with ordinal less than 1.");
            if (ArrivalTimestamp > DateTime.UtcNow)
                throw new EntityValidationException("Arrival timestamp must be in the past.");
        }

        private static DateTime EnsureUtc(DateTime value)
        {
            if (value.Kind == DateTimeKind.Utc)
                return value;
            else
                return DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }
}
