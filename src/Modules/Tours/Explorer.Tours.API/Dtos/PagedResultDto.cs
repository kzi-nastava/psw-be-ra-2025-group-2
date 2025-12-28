using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class PagedResultDto<T>
    {
        public List<T> Results { get; set; } = new();
        public int TotalCount { get; set; }
    }

}
