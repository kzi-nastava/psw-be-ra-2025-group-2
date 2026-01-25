using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Stakeholders.Core.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.Planner
{
    public enum EvaluationTimeScope
    {
        Day,
        Month
    }
    public class PlanEvaluationContext
    {
        public EvaluationTimeScope Scope { get; private set; }
        public DateOnly Date { get; private set; }

        private List<PlanEvaluationEntry> _entries = new();

        public IReadOnlyList<PlanEvaluationEntry> Entries => _entries.AsReadOnly();

        private delegate bool ScopeCriteria(DateTimeInterval interval, DateOnly date);

        private PlanEvaluationContext(EvaluationTimeScope scope, DateOnly date, List<PlanEvaluationEntry> entries)
        {
            Scope = scope;
            Date = date;
            _entries = entries;

            Validate();
        }

        private void Validate()
        {
            ScopeCriteria criterion = (Scope == EvaluationTimeScope.Day) ? IsDayEqual : AreMonthsEqual;
            foreach(var entry in _entries)
            {
                if (!criterion(entry.Slot, Date))
                    throw new PlanEvaluationContextException($"Invalid evaluation context.");
            }

        }

        private bool IsDayEqual(DateTimeInterval interval, DateOnly date)
        {
            return DateOnly.FromDateTime(interval.Start) == date && DateOnly.FromDateTime(interval.End) == date;
        }

        private bool AreMonthsEqual(DateTimeInterval interval, DateOnly date)
        {
            var monthStart = interval.Start.Month;
            var monthEnd = interval.End.Month;
            var yearStart = interval.Start.Year;
            var yearEnd = interval.End.Year;

            return monthStart == date.Month && monthEnd == date.Month && yearStart == date.Year && yearEnd == date.Year;
        }


        public class PlanEvaluationContextBuilder
        {
            private EvaluationTimeScope _scope;
            private DateOnly _date;
            private List<PlanEvaluationEntry> _entries = new();

            public PlanEvaluationContextBuilder() { }

            public PlanEvaluationContextBuilder WithTimeScope(EvaluationTimeScope scope)
            {
                _scope = scope;

                return this;
            }

            public PlanEvaluationContextBuilder ForDate(DateOnly date)
            {
                _date = date;

                return this;
            }

            public PlanEvaluationContextBuilder AddEntry(PlanEvaluationEntry entry)
            {
                _entries.Add(entry);

                return this;
            }

            public PlanEvaluationContext Build()
            {
                return new PlanEvaluationContext(_scope, _date, _entries);
            }
        }

    }
}
