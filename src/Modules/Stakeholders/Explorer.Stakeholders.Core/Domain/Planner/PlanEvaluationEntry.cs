using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Stakeholders.Core.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.Planner
{
    public class PlanEvaluationEntry : ValueObject
    {
        public string TourName { get; private set; }
        public DateTimeInterval Slot { get; private set; }
        public Minutes Minutes { get; private set; }
        public GeoLocation FirstKeyPointCoordinates { get; private set; }
        public GeoLocation LastKeyPointCoordinates { get; private set; }

        public PlanEvaluationEntry(string tourName, DateTimeInterval slot, Minutes minutes, GeoLocation firstKeyPointCoordinates, GeoLocation lastKeyPointCoordinates)
        {
            TourName = tourName;
            Slot = slot;
            Minutes = minutes;
            FirstKeyPointCoordinates = firstKeyPointCoordinates;
            LastKeyPointCoordinates = lastKeyPointCoordinates;

            Validate();
        }

        private void Validate()
        {
            if (Slot.Start.Date != Slot.End.Date)
                throw new ScheduleException("Invalid schedule entry.");

            if (Minutes < 0)
                throw new ArgumentException("Invalid tour duration.");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TourName;
            yield return Slot;
            yield return Minutes;
            yield return FirstKeyPointCoordinates;
            yield return LastKeyPointCoordinates;
        }
    }
}
