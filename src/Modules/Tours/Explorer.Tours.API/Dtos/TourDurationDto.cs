using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;

namespace Explorer.Tours.API.Dtos
{
    public enum TransportTypeDto
    {
        Walking = 0,   // peške
        Bicycle = 1,   // bicikl
        Car = 2        // automobil
    }

    public class TourDurationDto
    {
        public TransportTypeDto TransportType { get; set; }
        public int Minutes { get; set; }
    }
}
