using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Stakeholders.Core.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.Planner
{
    public class DayEntry : AggregateRoot
    {
        public long TouristId { get; private set; }
        public DateOnly Date { get; private set; }
        public string? Notes { get; private set; }

        private List<TourEntry> _entries = new();

        public IReadOnlyList<TourEntry> Entries => _entries.OrderBy(e => e.ScheduledTime.Start).ToList().AsReadOnly();

        private DayEntry() { }

        public DayEntry(long touristId, DateOnly date, string? notes)
        {
            TouristId = touristId;
            Date = date;
            Notes = notes;
        }

        public void SetNotes(string notes)
        {
            Notes = notes;
        }

        public void AddSchedule(long tourId, string? notes, DateTimeInterval scheduledTime)
        {
            if(scheduledTime == null)
                throw new ArgumentNullException(nameof(scheduledTime));

            ValidateSchedule(scheduledTime);

            _entries.Add(new TourEntry(tourId, notes, scheduledTime));
        }

        public void UpdateSchedule(long scheduleId, string? notes, DateTimeInterval scheduledTime)
        {
            var entry = _entries.Where(e => e.Id == scheduleId).FirstOrDefault();

            if (entry == null)
                throw new ArgumentException("Invalid schedule Id.");

            ValidateSchedule(scheduledTime);

            entry.SetNotes(notes);
            entry.SetScheduledTime(scheduledTime);
        }

        public void RemoveSchedule(long scheduleId)
        {
            var entry = _entries.Where(e => e.Id == scheduleId).FirstOrDefault();

            if (entry == null)
                throw new ArgumentException("Schedule not found.");

            _entries.Remove(entry);
        }

        private void ValidateSchedule(DateTimeInterval scheduledTime)
        {
            if (scheduledTime.Start.Day != scheduledTime.End.Day)
                throw new ScheduleException("The new schedule time must belong to the same day.");

            foreach (var entry in _entries)
            {
                if (!DateTimeInterval.AreDisjoint(scheduledTime, entry.ScheduledTime))
                    throw new ScheduleException("The new schedule time overlaps with one or more existing schedules.");
            }
        }
    }
}
