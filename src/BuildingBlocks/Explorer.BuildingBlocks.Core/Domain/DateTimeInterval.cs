using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.BuildingBlocks.Core.Domain
{
    public class DateTimeInterval : ValueObject
    {
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        public TimeSpan Duration => End - Start;

        private DateTimeInterval() { }

        private DateTimeInterval(DateTime start, DateTime end)
        {
            if (start >= end)
                throw new ArgumentException("Start date must be strictly earlier than the end date.");

            Start = EnsureUtc(start);
            End = EnsureUtc(end);
        }

        public static DateTimeInterval Of(DateTime start, DateTime end)
        {
            return new DateTimeInterval(start, end);
        }

        public bool Contains(DateTime instant)
        {
            return Start <= instant && End >= instant;
        }

        public bool Intersects(DateTimeInterval other)
        {
            return Start < other.End && other.Start < End;
        }

        public static bool AreDisjoint(DateTimeInterval first, DateTimeInterval second)
        {
            return !first.Intersects(second);
        }

        public DateTimeInterval Offset(TimeSpan span)
        {
            return new DateTimeInterval(Start + span, End + span);
        }

        private static DateTime EnsureUtc(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime;

            if (dateTime.Kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

            return dateTime.ToUniversalTime();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Start;
            yield return End;
        }
    }
}
