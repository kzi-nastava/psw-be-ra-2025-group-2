using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.Planner
{
    public enum EvaluationKind
    {
        None,
        SmallTimeSlot,
        Distance,
        OverbookedSchedule
    }
    public class PlanEvaluationResult : ValueObject
    {
        public EvaluationKind Kind { get; private set; }
        public DateOnly Date { get; private set; }
        public string? Message { get; private set; }

        private PlanEvaluationResult(EvaluationKind kind, DateOnly date, string message)
        {
            Kind = kind;
            Date = date;
            Message = message;
        }

        public static PlanEvaluationResult WithSmallTimeSlotIssue(DateOnly date, string tourName, Minutes minTourDurationMinutes, DateTimeInterval reservedSlot)
        {
            EvaluationKind kind = EvaluationKind.SmallTimeSlot;
            string message = $"Date: {date} - The tour \"{tourName}\" has a set duration of {minTourDurationMinutes}. You reserved a slot of {reservedSlot.Duration.Minutes} minutes. Consider extending the slot.";

            return new PlanEvaluationResult(kind, date, message);
        }

        public static PlanEvaluationResult WithDistanceIssue(DateOnly date, string firstTourName, string secondTourName, Distance distance)
        {
            EvaluationKind kind = EvaluationKind.Distance;
            string message = $"Date: {date} - The distance between the first key points of \"{firstTourName}\" and \"{secondTourName}\" is {distance.ToKilometers()} km. Consider a larger time gap between the two.";

            return new PlanEvaluationResult(kind, date, message);
        }

        public static PlanEvaluationResult WithOverbookingIssue(DateOnly date, IEnumerable<string> tours)
        {
            EvaluationKind kind = EvaluationKind.OverbookedSchedule;
            string message = $"Date: {date} - Your schedule for the day is too ambitious. Consider rescheduling your tours.";

            return new PlanEvaluationResult(kind, date, message);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            throw new NotImplementedException();
        }
    }
}
