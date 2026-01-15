using AutoMapper;
using Explorer.Encounters.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Mappers.Converters
{
    public class EncounterTypeConverter : ITypeConverter<string, EncounterType>
    {
        public EncounterType Convert(string source, EncounterType destination, ResolutionContext context)
        {
            return EncounterTypeParser.Parse(source);
        }
    }
}
