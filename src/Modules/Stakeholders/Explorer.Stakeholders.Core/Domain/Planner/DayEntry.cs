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

        private List<ScheduleEntry> _entries = new();

        public IReadOnlyList<ScheduleEntry> Entries => _entries.OrderBy(e => e.ScheduledTime.Start).ToList().AsReadOnly();

        private DayEntry() { }

        public DayEntry(long touristId, DateOnly date, string? notes)
        {
            TouristId = touristId;
            Date = date;
            Notes = notes;
        }

        public void SetNotes(string? notes)
        {
            Notes = notes;
        }

        public void AddScheduleEntry(long tourId, string? notes, DateTimeInterval scheduledTime)
        {
            if(scheduledTime == null)
                throw new ArgumentNullException(nameof(scheduledTime));

            ValidateSchedule(scheduledTime);

            _entries.Add(new ScheduleEntry(tourId, notes, scheduledTime));
        }

        public void UpdateScheduleEntry(long scheduleEntryId, string? notes, DateTimeInterval scheduledTime, long tourId)
        {
            var entry = _entries.Where(e => e.Id == scheduleEntryId).FirstOrDefault();

            if (entry == null)
                throw new ArgumentException("Invalid schedule Id.");

            entry.SetNotes(notes);
            entry.SetScheduledTime(scheduledTime);
            entry.SetTourId(tourId);

            _entries.Remove(_entries.Where(x => x.Id == entry.Id).First());

            ValidateSchedule(scheduledTime);

            _entries.Add(entry);
        }

        public void RemoveScheduleEntry(long scheduleEntryId)
        {
            var entry = _entries.Where(e => e.Id == scheduleEntryId).FirstOrDefault();

            if (entry == null)
                throw new ArgumentException("Schedule not found.");

            _entries.Remove(entry);
        }

        private void ValidateSchedule(DateTimeInterval scheduledTime)
        {
            if (Date != DateOnly.FromDateTime(scheduledTime.Start) || Date != DateOnly.FromDateTime(scheduledTime.End))
                throw new ScheduleException("The new schedule must be defined within the same day.");

            foreach (var entry in _entries)
            {
                if (!DateTimeInterval.AreDisjoint(scheduledTime, entry.ScheduledTime))
                    throw new ScheduleException("The new schedule time overlaps with one or more existing schedules.");
            }
        }
    }
}
