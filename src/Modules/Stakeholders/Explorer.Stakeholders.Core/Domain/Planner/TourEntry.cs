using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.Planner
{
    public class TourEntry : Entity
    {
        public long TourId { get; private set; }
        public string? Notes { get; private set; }
        public DateTimeInterval ScheduledTime { get; private set; }

        private TourEntry() { }

        public TourEntry(long tourId, string? notes, DateTimeInterval scheduledTime)
        {
            TourId = tourId;
            Notes = notes;
            ScheduledTime = scheduledTime;
        }

        public void SetNotes(string? notes)
        {
            Notes = notes;
        }

        public void SetScheduledTime(DateTimeInterval scheduledTime)
        {
            if (scheduledTime == null)
                throw new ArgumentNullException(nameof(scheduledTime));

            ScheduledTime = scheduledTime;
        }
    }
}
