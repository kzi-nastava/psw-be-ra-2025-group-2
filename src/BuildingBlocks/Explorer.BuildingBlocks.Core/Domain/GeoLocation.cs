using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.BuildingBlocks.Core.Domain
{
    public class GeoLocation : ValueObject
    {
        public double Latitude { get; init; }
        public double Longitude { get; init; }

        private GeoLocation() { }

        public GeoLocation(double latitude, double longitude)
        {
            if (latitude < -90.0 || latitude > 90.0)
                throw new ArgumentOutOfRangeException(nameof(latitude), $"Invalid latitude: {latitude}");

            if (longitude < -180.0 || longitude > 180.0)
                throw new ArgumentOutOfRangeException(nameof(longitude), $"Invalid longitude: {longitude}");

            Latitude = latitude;
            Longitude = longitude;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Math.Round(Latitude, 6);
            yield return Math.Round(Longitude, 6);
        }
    }
}
