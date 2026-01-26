using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Stakeholders.Core.Domain.Exceptions;
using Explorer.Stakeholders.Core.Domain.Planner;
using Shouldly;
using static Explorer.Stakeholders.Core.Domain.Planner.PlanEvaluationContext;

namespace Explorer.Stakeholders.Tests.Unit
{

    public class PlanEvaluationContextTests
    {
        private readonly DateTime _targetStart = new(2026, 2, 13, 10, 0, 0);
        private readonly DateTime _targetEnd = new(2026, 2, 13, 12, 0, 0);

        [Fact]
        public void Build_WithValidDayScope_ShouldSucceed()
        {
            var entry = CreateValidEntry(_targetStart, _targetEnd);

            var context = new PlanEvaluationContextBuilder()
                .WithTimeScope(EvaluationTimeScope.Day)
                .ForDate(DateOnly.FromDateTime(_targetStart))
                .AddEntry(entry)
                .Build();

            context.ShouldNotBeNull();
            context.Scope.ShouldBe(EvaluationTimeScope.Day);
            context.Entries.Count.ShouldBe(1);
            context.Entries.ShouldContain(entry);
        }

        [Fact]
        public void Build_WithValidMonthScope_ShouldSucceed()
        {
            var sameMonthDate = new DateTime(2026, 2, 15, 10, 0, 0);
            var entry = CreateValidEntry(sameMonthDate, sameMonthDate.AddHours(3));

            var context = new PlanEvaluationContextBuilder()
                .WithTimeScope(EvaluationTimeScope.Month)
                .ForDate(DateOnly.FromDateTime(sameMonthDate))
                .AddEntry(entry)
                .Build();

            context.Scope.ShouldBe(EvaluationTimeScope.Month);
            context.Date.Month.ShouldBe(sameMonthDate.Month);
        }

        [Fact]
        public void Build_WhenEntryDayDoesNotMatchDayScope_ShouldThrowException()
        {
            var differentDate = _targetEnd.AddDays(1);
            var entry = CreateValidEntry(differentDate, differentDate.AddHours(3));

            var action = () => new PlanEvaluationContextBuilder()
                .WithTimeScope(EvaluationTimeScope.Day)
                .ForDate(DateOnly.FromDateTime(_targetStart))
                .AddEntry(entry)
                .Build();

            action.ShouldThrow<PlanEvaluationContextException>().Message.ShouldBe("Invalid evaluation context.");
        }

        [Fact]
        public void Build_WhenEntryMonthDoesNotMatchMonthScope_ShouldThrowException()
        {
            var differentMonth = _targetStart.AddMonths(1);
            var entry = CreateValidEntry(differentMonth, differentMonth.AddHours(3));

            var action = () => new PlanEvaluationContextBuilder()
                .WithTimeScope(EvaluationTimeScope.Month)
                .ForDate(DateOnly.FromDateTime(_targetStart))
                .AddEntry(entry)
                .Build();

            action.ShouldThrow<PlanEvaluationContextException>().Message.ShouldBe("Invalid evaluation context.");
        }

        private PlanEvaluationEntry CreateValidEntry(DateTime start, DateTime end)
        {
            var interval = DateTimeInterval.Of(start, end);

            return new PlanEvaluationEntry(
                "Test Tour",
                interval,
                Minutes.Of(interval.Duration.Minutes),
                new GeoLocation(44.7866, 20.4489),
                new GeoLocation(44.7870, 20.4500)
            );
        }
    }
}
