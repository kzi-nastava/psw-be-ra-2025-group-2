using Explorer.BuildingBlocks.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.Exceptions
{
    public class TourExecutionStateException : DomainException
    {
        public TourExecutionStateException(string message) : base(message)
        {

        }
    }
}
