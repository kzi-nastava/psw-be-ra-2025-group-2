using System;
using System.Collections.Generic;
using System.Linq;
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

        private List<PlanEvaluationEntry> _entries;

        public IReadOnlyList<PlanEvaluationEntry> Entries => _entries.AsReadOnly();

        public class PlanEvaluationContextBuilder
        {

        }
    }
}
