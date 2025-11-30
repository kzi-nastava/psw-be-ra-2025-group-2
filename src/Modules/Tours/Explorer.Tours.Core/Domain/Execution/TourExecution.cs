using Explorer.BuildingBlocks.Core.Domain;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Exceptions;
using System.Runtime;
using Explorer.Tours.Core.Domain.Exceptions;

namespace Explorer.Tours.Core.Domain.Execution

{
    public class TourExecution : AggregateRoot
    {
        public int TouristId { get; init; }
        public int TourId { get; init; }

        private readonly List<KeyPointVisit> _keyPointVisits;
        public IReadOnlyList<KeyPointVisit> KeyPointVisits => _keyPointVisits.AsReadOnly();
        public TourExecutionState State { get; private set; } = TourExecutionState.NotStarted;

        public DateTime LastActivityTimestamp { get; private set; }
        public DateTime? CompletionTimestamp { get; private set; }


        public TourExecution(int touristId, int tourId, List<KeyPointVisit> keyPointVisits, TourExecutionState state, DateTime lastActivityTimestamp, DateTime? completionTimestamp)
        {
            LastActivityTimestamp = EnsureUtc(lastActivityTimestamp);
            CompletionTimestamp = completionTimestamp == null ? null : EnsureUtc(completionTimestamp.Value);

            TouristId = touristId;
            TourId = tourId;
            _keyPointVisits = (keyPointVisits ?? new List<KeyPointVisit>()).OrderBy(kpv => kpv.ArrivalTimestamp).ToList();
            State = state;
            LastActivityTimestamp = lastActivityTimestamp;
            CompletionTimestamp = completionTimestamp;
            Validate();
        }


        public void Start()
        {
            if (State != TourExecutionState.NotStarted)
                throw new TourExecutionStateException("Cannot initiate a tour execution more than once.");

            State = TourExecutionState.InProgress;
            RecordActivity();
        }

        public void Abandon()
        {
            if(State != TourExecutionState.InProgress)
                throw new TourExecutionStateException("Cannot abandon a tour which is not in progress.");

            State = TourExecutionState.Abandoned;
            CompletionTimestamp = DateTime.UtcNow;
        }

        public void Complete()
        {
            if (State != TourExecutionState.InProgress)
                throw new TourExecutionStateException("Cannot complete a tour which is not in progress.");

            State = TourExecutionState.Completed;
            CompletionTimestamp = DateTime.UtcNow;
        }

        public int GetLastVisitedKeyPointOrdinal()
        {
            return _keyPointVisits.LastOrDefault()?.KeyPointOrdinal ?? 0;
        }

        public void ArriveAtKeyPointByOrdinal(int ordinal)
        {
            if(ordinal < 1)
                throw new ArgumentException("Invalid key point ordinal.");

            int lastVisited = GetLastVisitedKeyPointOrdinal();

            if (ordinal != lastVisited + 1)
                throw new KeyPointVisitException($"Cannot visit key point #{ordinal} after #{lastVisited}");

            _keyPointVisits.Add(new KeyPointVisit(ordinal, DateTime.UtcNow));

        }

        public void RecordActivity()
        {
            LastActivityTimestamp = DateTime.UtcNow;
        }

        private void Validate()
        {
            if(LastActivityTimestamp > DateTime.UtcNow)
                throw new EntityValidationException("Last activity must be in the past.");

            if (CompletionTimestamp.HasValue && CompletionTimestamp.Value > DateTime.UtcNow)
                throw new EntityValidationException("Completion time must be in the past.");
        }


        private static DateTime EnsureUtc(DateTime value)
        {
            if (value.Kind == DateTimeKind.Utc)
                return value;
            else
                return DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }


    public enum TourExecutionState
    {
        NotStarted = 0,
        InProgress = 1,
        Completed = 2,
        Abandoned = 3
    }
}
