using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Stakeholders.Tests.Unit
{
    public class DateTimeIntervalTests
    {
        [Fact]
        public void Creates_valid_interval()
        {
            var start = new DateTime(2026, 1, 1);
            var end = new DateTime(2026, 1, 2);

            var interval = DateTimeInterval.Of(start, end);

            interval.Start.ShouldBe(start);
            interval.End.ShouldBe(end);
        }

        [Fact]
        public void Throws_exception_when_start_is_after_end()
        {
            var start = new DateTime(2026, 1, 2);
            var end = new DateTime(2026, 1, 1);

            Should.Throw<ArgumentException>(() => DateTimeInterval.Of(start, end));
        }

        [Fact]
        public void Throws_exception_when_start_equals_end()
        {
            var time = new DateTime(2026, 1, 1);

            Should.Throw<ArgumentException>(() => DateTimeInterval.Of(time, time));
        }

        [Theory]
        [InlineData("2026-01-01", "2026-01-05", "2026-01-03", true)]   // Inside
        [InlineData("2026-01-01", "2026-01-05", "2026-01-01", true)]   // On Start boundary
        [InlineData("2026-01-01", "2026-01-05", "2026-01-05", true)]   // On End boundary
        [InlineData("2026-01-01", "2026-01-05", "2025-12-31", false)]  // Before
        [InlineData("2026-01-01", "2026-01-05", "2026-01-06", false)]  // After
        public void Checks_if_contains_instant(string startStr, string endStr, string checkStr, bool expected)
        {
            var interval = DateTimeInterval.Of(DateTime.Parse(startStr), DateTime.Parse(endStr));
            var instant = DateTime.Parse(checkStr);

            interval.Contains(instant).ShouldBe(expected);
        }

        [Fact]
        public void Intersects_correctly()
        {
            var main = DateTimeInterval.Of(new DateTime(2026, 1, 10), new DateTime(2026, 1, 20));

            // Overlapping
            var overlapping = DateTimeInterval.Of(new DateTime(2026, 1, 15), new DateTime(2026, 1, 25));
            main.Intersects(overlapping).ShouldBeTrue();

            // Disjoint (Before)
            var before = DateTimeInterval.Of(new DateTime(2026, 1, 1), new DateTime(2026, 1, 5));
            main.Intersects(before).ShouldBeFalse();

            // Disjoint (After)
            var after = DateTimeInterval.Of(new DateTime(2026, 1, 25), new DateTime(2026, 1, 30));
            main.Intersects(after).ShouldBeFalse();
        }

        [Fact]
        public void AreDisjoint_returns_true_for_separated_intervals()
        {
            var first = DateTimeInterval.Of(new DateTime(2026, 1, 1), new DateTime(2026, 1, 10));
            var second = DateTimeInterval.Of(new DateTime(2026, 1, 11), new DateTime(2026, 1, 20));

            DateTimeInterval.AreDisjoint(first, second).ShouldBeTrue();
        }

        [Fact]
        public void Value_object_equality_works()
        {
            var start = new DateTime(2026, 1, 1);
            var end = new DateTime(2026, 1, 10);

            var interval1 = DateTimeInterval.Of(start, end);
            var interval2 = DateTimeInterval.Of(start, end);

            interval1.ShouldBe(interval2); // Tests EqualityComponents
        }
    }
}
