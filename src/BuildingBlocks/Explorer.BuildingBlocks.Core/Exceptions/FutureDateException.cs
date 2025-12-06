using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.BuildingBlocks.Core.Exceptions
{
    public class FutureDateException : DomainException
    {
        public FutureDateException(string message) : base(message)
        {

        }
    }
}
