using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Tests.Unit
{
    using Explorer.BuildingBlocks.Core.Domain;
    using Explorer.Stakeholders.Core.Domain.Planner;
    using Shouldly;
    using Xunit;
    using static Explorer.Stakeholders.Core.Domain.Planner.PlanEvaluationContext;

    public class PlanEvaluatorTests
    {
        private readonly PlanEvaluator _evaluator = new();
        private readonly GeoLocation _locationA = new(44.7866, 20.4489);
        private readonly GeoLocation _locationB = new(44.8186, 20.4689); // ~4km away

        [Fact]
        public void Evaluate_WithSmallTimeSlot_ShouldReturnSmallTimeSlotIssue()
        {
            var date = new DateOnly(2026, 2, 13);
            var start = new DateTime(2026, 2, 13, 10, 0, 0);
            // Tour is 100 mins, but slot is only 60 mins (60 < 0.8 * 100)
            var entry = CreateEntry("Quick Tour", start, start.AddMinutes(60), 100, _locationA, _locationA);

            var ctx = new PlanEvaluationContextBuilder()
                .WithTimeScope(EvaluationTimeScope.Day)
                .ForDate(date)
                .AddEntry(entry)
                .Build();

            var results = _evaluator.Evaluate(ctx).ToList();

            results.ShouldContain(r => r.Kind == EvaluationKind.SmallTimeSlot);
            results.First().Message.ShouldContain("Consider extending the slot");
        }

        [Fact]
        public void Evaluate_WithHighVelocityRequirement_ShouldReturnDistanceIssue()
        {
            var date = new DateOnly(2026, 2, 13);
            var tour1End = new DateTime(2026, 2, 13, 10, 0, 0);
            // Tour 2 starts exactly when Tour 1 ends (0 minutes gap)
            var tour2Start = tour1End;

            var entry1 = CreateEntry("Tour 1", tour1End.AddHours(-1), tour1End, 60, _locationA, _locationA);
            var entry2 = CreateEntry("Tour 2", tour2Start, tour2Start.AddHours(1), 60, _locationB, _locationB);

            var ctx = new PlanEvaluationContextBuilder()
                .WithTimeScope(EvaluationTimeScope.Day)
                .ForDate(date)
                .AddEntry(entry1)
                .AddEntry(entry2)
                .Build();

            var results = _evaluator.Evaluate(ctx).ToList();

            results.ShouldContain(r => r.Kind == EvaluationKind.Distance);
        }

        [Fact]
        public void Evaluate_WithBackToBackTours_WithinVelocityLimit_ShouldBeValid()
        {
            var date = new DateOnly(2026, 2, 13);
            var tour1End = new DateTime(2026, 2, 13, 10, 0, 0);
            var tour2Start = tour1End; // Back-to-back

            // Same location, so velocity required is 0
            var entry1 = CreateEntry("Tour 1", tour1End.AddHours(-1), tour1End, 60, _locationA, _locationA);
            var entry2 = CreateEntry("Tour 2", tour2Start, tour2Start.AddHours(1), 60, _locationA, _locationA);

            var ctx = new PlanEvaluationContextBuilder()
                .WithTimeScope(EvaluationTimeScope.Day)
                .ForDate(date)
                .AddEntry(entry1)
                .AddEntry(entry2)
                .Build();

            var results = _evaluator.Evaluate(ctx);

            results.ShouldBeEmpty();
        }

        [Fact]
        public void Evaluate_WithMoreThanThreeToursInDay_ShouldReturnOverbookedIssue()
        {
            var date = new DateOnly(2026, 2, 13);
            var builder = new PlanEvaluationContextBuilder()
                .WithTimeScope(EvaluationTimeScope.Day)
                .ForDate(date);

            for (int i = 0; i < 4; i++)
            {
                var start = new DateTime(2026, 2, 13, 8 + (i * 3), 0, 0);
                builder.AddEntry(CreateEntry($"Tour {i}", start, start.AddHours(2), 120, _locationA, _locationA));
            }

            var results = _evaluator.Evaluate(builder.Build()).ToList();

            results.ShouldContain(r => r.Kind == EvaluationKind.OverbookedSchedule);
        }

        [Fact]
        public void Evaluate_DistanceConflictInterval_ShouldSuppressConsecutiveRapidConflicts()
        {
            var date = new DateOnly(2026, 2, 13);
            var start = new DateTime(2026, 2, 13, 8, 0, 0);

            // Three tours very close together in space but requiring high velocity
            // The second conflict is within 6 hours of the first
            var entry1 = CreateEntry("T1", start, start.AddMinutes(30), 30, _locationA, _locationA);
            var entry2 = CreateEntry("T2", start.AddMinutes(31), start.AddMinutes(61), 30, _locationB, _locationB);
            var entry3 = CreateEntry("T3", start.AddMinutes(62), start.AddMinutes(92), 30, _locationA, _locationA);

            var ctx = new PlanEvaluationContextBuilder()
                .WithTimeScope(EvaluationTimeScope.Day)
                .ForDate(date)
                .AddEntry(entry1)
                .AddEntry(entry2)
                .AddEntry(entry3)
                .Build();

            var results = _evaluator.Evaluate(ctx).ToList();

            // Should only have 1 Distance issue because of MinDistanceConflictInterval (6 hours)
            results.Count(r => r.Kind == EvaluationKind.Distance).ShouldBe(1);
        }

        [Fact]
        public void Evaluate_WithChainOfBackToBackConflicts_ShouldOnlyReportOneConflict()
        {
            var date = new DateOnly(2026, 2, 13);
            var startTime = new DateTime(2026, 2, 13, 8, 0, 0);
            var builder = new PlanEvaluationContextBuilder()
                .WithTimeScope(EvaluationTimeScope.Day)
                .ForDate(date);

            // Create 6 tours (forming 5 back-to-back pairs)
            // Each pair is separated by 0 minutes and ~4km, triggering the -1 velocity logic
            for (int i = 0; i < 6; i++)
            {
                var start = startTime.AddMinutes(i * 61); // 60 min tour + 1 min gap (or 0)
                var end = start.AddMinutes(60);

                // Toggle locations so every transition is a distance conflict
                var loc = (i % 2 == 0) ? _locationA : _locationB;

                builder.AddEntry(CreateEntry($"Tour {i}", start, end, 60, loc, loc));
            }

            var results = _evaluator.Evaluate(builder.Build()).ToList();

            // Despite 5 potential conflicts, only 1 should be reported 
            // because they all fall within the 6-hour suppression window from the first conflict
            results.Count(r => r.Kind == EvaluationKind.Distance).ShouldBe(1);
        }

        [Fact]
        public void Evaluate_WithConflictsSeparatedByMoreThanSixHours_ShouldReportMultipleConflicts()
        {
            var date = new DateOnly(2026, 2, 13);
            var builder = new PlanEvaluationContextBuilder()
                .WithTimeScope(EvaluationTimeScope.Day)
                .ForDate(date);

            // Conflict 1: Morning (8:00 - 9:00)
            var t1End = new DateTime(2026, 2, 13, 9, 0, 0);
            builder.AddEntry(CreateEntry("T1", t1End.AddHours(-1), t1End, 60, _locationA, _locationA));
            builder.AddEntry(CreateEntry("T2", t1End, t1End.AddHours(1), 60, _locationB, _locationB));

            // Conflict 2: Evening (16:00 - 17:00)
            // The difference between T3.End (16:00) and T1.End (9:00) is 7 hours (> 6 hours)
            var t3End = new DateTime(2026, 2, 13, 16, 0, 0);
            builder.AddEntry(CreateEntry("T3", t3End.AddHours(-1), t3End, 60, _locationA, _locationA));
            builder.AddEntry(CreateEntry("T4", t3End, t3End.AddHours(1), 60, _locationB, _locationB));

            var results = _evaluator.Evaluate(builder.Build()).ToList();

            // Should report 2 conflicts because the time elapsed since the last reported conflict end is > 6 hours
            results.Count(r => r.Kind == EvaluationKind.Distance).ShouldBe(2);
            results.Where(r => r.Kind == EvaluationKind.Distance).ShouldAllBe(r => r.Date == date);
        }

        private PlanEvaluationEntry CreateEntry(string name, DateTime start, DateTime end, int minDuration, GeoLocation first, GeoLocation last)
        {
            return new PlanEvaluationEntry(
                name,
                DateTimeInterval.Of(start, end),
                Minutes.Of(minDuration),
                first,
                last
            );
        }
    }
}
