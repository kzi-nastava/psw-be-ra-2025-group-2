using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.BuildingBlocks.Domain
{
    public static class Haversine
    {
        public static Distance CalculateDistance(GeoLocation loc1, GeoLocation loc2)
        {
            const double R = 6371;

            var deltaLat = ToRadians(loc2.Latitude - loc1.Latitude);
            var deltaLon = ToRadians(loc2.Longitude - loc2.Longitude);

            var lat1 = ToRadians(loc1.Latitude);
            var lat2 = ToRadians(loc2.Latitude);

            var a = Math.Pow(Math.Sin(deltaLat / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(deltaLon / 2), 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            var distance = R * c;

            return Distance.FromKilometers(distance);
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
