using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Stakeholders.Core.Domain.Exceptions;
using Explorer.Stakeholders.Core.Domain.Planner;
using Shouldly;
using Xunit;

namespace Explorer.Stakeholders.Tests.Unit
{
    public class PlannerUnitTests
    {
        [Fact]
        public void Creates_day_entry()
        {
            var date = new DateOnly(2026, 5, 20);
            var dayEntry = new DayEntry(100, date, "Initial plan");

            dayEntry.TouristId.ShouldBe(100);
            dayEntry.Date.ShouldBe(date);
            dayEntry.Notes.ShouldBe("Initial plan");
            dayEntry.Entries.ShouldBeEmpty();
        }

        [Fact]
        public void Adds_schedule_successfully()
        {
            var dayEntry = new DayEntry(1, new DateOnly(2026, 5, 20), null);
            var interval = DateTimeInterval.Of(new DateTime(2026, 5, 20, 10, 0, 0), new DateTime(2026, 5, 20, 12, 0, 0));

            dayEntry.AddScheduleEntry(50, "Visit Kalemegdan", interval);

            dayEntry.Entries.Count.ShouldBe(1);
            dayEntry.Entries[0].TourId.ShouldBe(50);
            dayEntry.Entries[0].ScheduledTime.ShouldBe(interval);
        }

        [Fact]
        public void Throws_exception_when_schedule_spans_multiple_days()
        {
            var dayEntry = new DayEntry(1, new DateOnly(2026, 5, 20), null);
            var multiDayInterval = DateTimeInterval.Of(
                new DateTime(2026, 5, 20, 22, 0, 0),
                new DateTime(2026, 5, 21, 01, 0, 0)
            );

            var exception = Should.Throw<ScheduleException>(() =>
                dayEntry.AddScheduleEntry(1, "Late night tour", multiDayInterval));

            exception.Message.ShouldBe("The new schedule must be defined within the same day.");
        }

        [Fact]
        public void Throws_exception_when_schedules_overlap()
        {
            var dayEntry = new DayEntry(1, new DateOnly(2026, 5, 20), null);
            var firstInterval = DateTimeInterval.Of(new DateTime(2026, 5, 20, 10, 0, 0), new DateTime(2026, 5, 20, 12, 0, 0));
            var overlappingInterval = DateTimeInterval.Of(new DateTime(2026, 5, 20, 11, 0, 0), new DateTime(2026, 5, 20, 13, 0, 0));

            dayEntry.AddScheduleEntry(1, "Morning Tour", firstInterval);

            var exception = Should.Throw<ScheduleException>(() =>
                dayEntry.AddScheduleEntry(2, "Overlapping Tour", overlappingInterval));

            exception.Message.ShouldBe("The new schedule time overlaps with one or more existing schedules.");
        }

        [Fact]
        public void Entries_are_ordered_by_start_time()
        {
            var dayEntry = new DayEntry(1, new DateOnly(2026, 5, 20), null);
            var morning = DateTimeInterval.Of(new DateTime(2026, 5, 20, 09, 0, 0), new DateTime(2026, 5, 20, 10, 0, 0));
            var evening = DateTimeInterval.Of(new DateTime(2026, 5, 20, 18, 0, 0), new DateTime(2026, 5, 20, 20, 0, 0));
            var noon = DateTimeInterval.Of(new DateTime(2026, 5, 20, 12, 0, 0), new DateTime(2026, 5, 20, 14, 0, 0));

            // Adding out of order
            dayEntry.AddScheduleEntry(1, "Evening", evening);
            dayEntry.AddScheduleEntry(2, "Morning", morning);
            dayEntry.AddScheduleEntry(3, "Noon", noon);

            dayEntry.Entries[0].ScheduledTime.Start.Hour.ShouldBe(9);
            dayEntry.Entries[1].ScheduledTime.Start.Hour.ShouldBe(12);
            dayEntry.Entries[2].ScheduledTime.Start.Hour.ShouldBe(18);
        }

        [Fact]
        public void Removes_schedule_successfully()
        {
            var dayEntry = new DayEntry(1, new DateOnly(2026, 5, 20), null);
            var interval = DateTimeInterval.Of(new DateTime(2026, 5, 20, 10, 0, 0), new DateTime(2026, 5, 20, 12, 0, 0));
            dayEntry.AddScheduleEntry(1, "Tour to remove", interval);

            // Assuming ID is assigned by the system or during AddSchedule (simulating with 0 for unpersisted entity)
            var entryId = dayEntry.Entries[0].Id;

            dayEntry.RemoveScheduleEntry(entryId);

            dayEntry.Entries.ShouldBeEmpty();
        }

        [Fact]
        public void Updates_notes_for_day()
        {
            var dayEntry = new DayEntry(1, new DateOnly(2026, 5, 20), "Old Notes");

            dayEntry.SetNotes("New updated notes");

            dayEntry.Notes.ShouldBe("New updated notes");
        }

        [Fact]
        public void Updates_existing_schedule_successfully()
        {
            // Arrange
            var dayEntry = new DayEntry(1, new DateOnly(2026, 5, 20), "Planning day");
            var originalInterval = DateTimeInterval.Of(new DateTime(2026, 5, 20, 10, 0, 0), new DateTime(2026, 5, 20, 11, 0, 0));
            dayEntry.AddScheduleEntry(50, "Original Notes", originalInterval);

            var entryId = dayEntry.Entries[0].Id;
            var newInterval = DateTimeInterval.Of(new DateTime(2026, 5, 20, 14, 0, 0), new DateTime(2026, 5, 20, 15, 0, 0));
            string newNotes = "Updated Notes";

            // Act
            dayEntry.UpdateScheduleEntry(entryId, newNotes, newInterval, -10);

            // Assert
            dayEntry.Entries[0].Notes.ShouldBe(newNotes);
            dayEntry.Entries[0].ScheduledTime.ShouldBe(newInterval);
            dayEntry.Entries[0].TourId.ShouldBe(-10);
        }

        [Fact]
        public void Update_does_not_throw_exception_if_new_time_overlaps_with_others()
        {
            // Arrange
            var dayEntry = new DayEntry(1, new DateOnly(2026, 5, 20), null);
            var morning = DateTimeInterval.Of(new DateTime(2026, 5, 20, 09, 0, 0), new DateTime(2026, 5, 20, 10, 0, 0));
            var afternoon = DateTimeInterval.Of(new DateTime(2026, 5, 20, 14, 0, 0), new DateTime(2026, 5, 20, 15, 0, 0));

            dayEntry.AddScheduleEntry(1, "Morning Tour", morning);
            dayEntry.AddScheduleEntry(2, "Afternoon Tour", afternoon);

            var afternoonId = dayEntry.Entries.First(e => e.TourId == 2).Id;
            var overlappingInterval = DateTimeInterval.Of(new DateTime(2026, 5, 20, 14, 30, 0), new DateTime(2026, 5, 20, 15, 30, 0));

            // Act & Assert
            Should.Throw<ScheduleException>(() =>
                dayEntry.UpdateScheduleEntry(afternoonId, "Trying to overlap", overlappingInterval, -50));
        }

        [Fact]
        public void Throws_exception_when_removing_non_existent_schedule()
        {
            // Arrange
            var dayEntry = new DayEntry(1, new DateOnly(2026, 5, 20), null);
            long nonExistentId = 999;

            // Act & Assert
            var exception = Should.Throw<ArgumentException>(() => dayEntry.RemoveScheduleEntry(nonExistentId));
            exception.Message.ShouldBe("Schedule not found.");
        }

        [Fact]
        public void EnsureUtc_converts_local_time_to_utc_correctly()
        {
            // Simulate a local time (e.g., Belgrade 10:00 AM)
            var localStart = new DateTime(2026, 5, 20, 10, 0, 0, DateTimeKind.Local);
            var localEnd = new DateTime(2026, 5, 20, 11, 0, 0, DateTimeKind.Local);

            var interval = DateTimeInterval.Of(localStart, localEnd);

            // It should be stored as UTC (Belgrade is UTC+2 in May 2026)
            interval.Start.Kind.ShouldBe(DateTimeKind.Utc);
            interval.Start.Hour.ShouldBe(8);
        }

        [Fact]
        public void Validation_works_across_utc_day_boundaries()
        {
            // Local Belgrade time: May 21st, 01:00 AM (which is May 20th, 11:00 PM UTC)
            // If our DayEntry is for May 20th, this SHOULD be allowed if using UTC consistently.
            var dayEntry = new DateOnly(2026, 5, 20);
            var planner = new DayEntry(1, dayEntry, null);

            var start = new DateTime(2026, 5, 20, 22, 0, 0, DateTimeKind.Utc); // 22:00 UTC
            var end = new DateTime(2026, 5, 20, 23, 0, 0, DateTimeKind.Utc);   // 23:00 UTC

            // Act & Assert
            Should.NotThrow(() => planner.AddScheduleEntry(1, "Late Tour", DateTimeInterval.Of(start, end)));
        }

        [Fact]
        public void Throws_when_utc_times_actually_cross_midnight()
        {
            var dayEntry = new DayEntry(1, new DateOnly(2026, 5, 20), null);

            // This is 23:00 UTC on the 20th to 01:00 UTC on the 21st
            var crossMidnight = DateTimeInterval.Of(
                new DateTime(2026, 5, 20, 23, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 5, 21, 1, 0, 0, DateTimeKind.Utc));

            Should.Throw<ScheduleException>(() => dayEntry.AddScheduleEntry(1, "Midnight Cross", crossMidnight));
        }
    }
}
