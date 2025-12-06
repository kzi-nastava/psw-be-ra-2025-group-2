using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class KeyPointVisitResponseDto
    {
        public bool IsNewVisit { get; set; }
        public int? KeyPointOrdinal { get; set; }
    }
}
