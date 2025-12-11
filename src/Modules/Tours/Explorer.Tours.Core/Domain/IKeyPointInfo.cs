using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public interface IKeyPointInfo
    {
        public int OrdinalNo { get; }
        public double Latitude { get; }
        public double Longitude { get; }
    }
}
