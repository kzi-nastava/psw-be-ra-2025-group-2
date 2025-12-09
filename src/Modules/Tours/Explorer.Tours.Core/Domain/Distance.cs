using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class Distance : ValueObject
    {
        public double Meters { get; }

        public static Distance Zero => new Distance(0);

        private Distance(double meters)
        {
            if (meters < 0)
                throw new ArgumentException("Distance cannot be negative.");

            Meters = meters;
        }

        public static Distance FromMeters(double meters)
        {
            return new Distance(meters);
        }

        public static Distance FromKilometers(double kilometers)
        {
            return new Distance(kilometers * 1000);
        }

        public double ToKilometers()
        {
            return Meters / 1000;
        }

        public static Distance operator+(Distance a, Distance b)
        {
            return new Distance(a.Meters + b.Meters);
        }

        public static Distance operator-(Distance a, Distance b)
        {
            var result = a.Meters - b.Meters;

            if (result < 0)
                throw new InvalidOperationException("Resulting distance cannot be negative.");

            return new Distance(result);
        }

        public static double operator/(Distance a, Distance b)
        {
            if (b == Zero)
                throw new InvalidOperationException("The divisor distance cannot be zero.");

            return a.Meters / b.Meters;
        }

        public static bool operator <(Distance a, Distance b) => a.Meters < b.Meters;
        public static bool operator >(Distance a, Distance b) => a.Meters > b.Meters;

        public static bool operator <=(Distance a, Distance b) => a.Meters <= b.Meters;
        public static bool operator >=(Distance a, Distance b) => a.Meters >= b.Meters;

        public static bool operator ==(Distance a, Distance b) => a.Equals(b);
        public static bool operator !=(Distance a, Distance b) => !(a == b);

        public static implicit operator double(Distance d) => d.Meters;
        public static explicit operator Distance(double meters) => FromMeters(meters);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Math.Round(Meters, 6);
        }
    }
}
