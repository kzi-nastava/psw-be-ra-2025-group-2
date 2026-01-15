using Explorer.BuildingBlocks.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain.Exceptions
{
    public class EncounterTypeParseException : DomainException
    {
        public EncounterTypeParseException(string message) : base(message) { }
    }
}
