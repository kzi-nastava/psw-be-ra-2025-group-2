using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public enum TransportType
    {
        Walking = 0,
        Bicycle = 1,
        Car = 2
    }
    public class TourDuration : ValueObject
    {
        public TransportType TransportType { get; private set; }
        public int Minutes { get; private set; }

        private TourDuration() { }

        public TourDuration(TransportType transportType, int minutes)
        {
            if (minutes <= 0)
                throw new ArgumentOutOfRangeException(nameof(minutes), "Duration must be positive.");

            TransportType = transportType;
            Minutes = minutes;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TransportType;
            yield return Minutes;
        }
    }
}
