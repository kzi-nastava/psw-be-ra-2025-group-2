using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.Planner.Services
{
    public interface IPlanEvaluator
    {
        IEnumerable<PlanEvaluationResult> Evaluate(PlanEvaluationContext ctx);
    }
}
