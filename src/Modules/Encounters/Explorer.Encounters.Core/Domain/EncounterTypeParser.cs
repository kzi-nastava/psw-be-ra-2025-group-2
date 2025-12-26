using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Encounters.Core.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain
{
    public static class EncounterTypeParser
    {
        public static EncounterType Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new EncounterTypeParseException("Encounter type is required.");

            if (!Enum.TryParse(value, ignoreCase: true, out EncounterType type))
                throw new EncounterTypeParseException($"Invalid encounter type {value}");

            return type;
        }
    }
}
