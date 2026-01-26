using Explorer.BuildingBlocks.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.Exceptions
{
    public class PlanEvaluationContextException : DomainException
    {
        public PlanEvaluationContextException(string message) : base(message)
        {
        }
    }
}
