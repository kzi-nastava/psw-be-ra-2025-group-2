using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class KeyPointImageDto
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public bool IsCover { get; set; }
        public int OrderIndex { get; set; }
    }
}
