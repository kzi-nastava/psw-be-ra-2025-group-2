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

            dayEntry.AddSchedule(50, "Visit Kalemegdan", interval);

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
                dayEntry.AddSchedule(1, "Late night tour", multiDayInterval));

            exception.Message.ShouldBe("The new schedule time must belong to the same day.");
        }

        [Fact]
        public void Throws_exception_when_schedules_overlap()
        {
            var dayEntry = new DayEntry(1, new DateOnly(2026, 5, 20), null);
            var firstInterval = DateTimeInterval.Of(new DateTime(2026, 5, 20, 10, 0, 0), new DateTime(2026, 5, 20, 12, 0, 0));
            var overlappingInterval = DateTimeInterval.Of(new DateTime(2026, 5, 20, 11, 0, 0), new DateTime(2026, 5, 20, 13, 0, 0));

            dayEntry.AddSchedule(1, "Morning Tour", firstInterval);

            var exception = Should.Throw<ScheduleException>(() =>
                dayEntry.AddSchedule(2, "Overlapping Tour", overlappingInterval));

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
            dayEntry.AddSchedule(1, "Evening", evening);
            dayEntry.AddSchedule(2, "Morning", morning);
            dayEntry.AddSchedule(3, "Noon", noon);

            dayEntry.Entries[0].ScheduledTime.Start.Hour.ShouldBe(9);
            dayEntry.Entries[1].ScheduledTime.Start.Hour.ShouldBe(12);
            dayEntry.Entries[2].ScheduledTime.Start.Hour.ShouldBe(18);
        }

        [Fact]
        public void Removes_schedule_successfully()
        {
            var dayEntry = new DayEntry(1, new DateOnly(2026, 5, 20), null);
            var interval = DateTimeInterval.Of(new DateTime(2026, 5, 20, 10, 0, 0), new DateTime(2026, 5, 20, 12, 0, 0));
            dayEntry.AddSchedule(1, "Tour to remove", interval);

            // Assuming ID is assigned by the system or during AddSchedule (simulating with 0 for unpersisted entity)
            var entryId = dayEntry.Entries[0].Id;

            dayEntry.RemoveSchedule(entryId);

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
            dayEntry.AddSchedule(50, "Original Notes", originalInterval);

            var entryId = dayEntry.Entries[0].Id;
            var newInterval = DateTimeInterval.Of(new DateTime(2026, 5, 20, 14, 0, 0), new DateTime(2026, 5, 20, 15, 0, 0));
            string newNotes = "Updated Notes";

            // Act
            dayEntry.UpdateSchedule(entryId, newNotes, newInterval);

            // Assert
            dayEntry.Entries[0].Notes.ShouldBe(newNotes);
            dayEntry.Entries[0].ScheduledTime.ShouldBe(newInterval);
        }

        [Fact]
        public void Update_throws_exception_if_new_time_overlaps_with_others()
        {
            // Arrange
            var dayEntry = new DayEntry(1, new DateOnly(2026, 5, 20), null);
            var morning = DateTimeInterval.Of(new DateTime(2026, 5, 20, 09, 0, 0), new DateTime(2026, 5, 20, 10, 0, 0));
            var afternoon = DateTimeInterval.Of(new DateTime(2026, 5, 20, 14, 0, 0), new DateTime(2026, 5, 20, 15, 0, 0));

            dayEntry.AddSchedule(1, "Morning Tour", morning);
            dayEntry.AddSchedule(2, "Afternoon Tour", afternoon);

            var afternoonId = dayEntry.Entries.First(e => e.TourId == 2).Id;
            var overlappingInterval = DateTimeInterval.Of(new DateTime(2026, 5, 20, 09, 30, 0), new DateTime(2026, 5, 20, 10, 30, 0));

            // Act & Assert
            Should.Throw<ScheduleException>(() =>
                dayEntry.UpdateSchedule(afternoonId, "Trying to overlap", overlappingInterval));
        }

        [Fact]
        public void Throws_exception_when_removing_non_existent_schedule()
        {
            // Arrange
            var dayEntry = new DayEntry(1, new DateOnly(2026, 5, 20), null);
            long nonExistentId = 999;

            // Act & Assert
            var exception = Should.Throw<ArgumentException>(() => dayEntry.RemoveSchedule(nonExistentId));
            exception.Message.ShouldBe("Schedule not found.");
        }
    }
}
