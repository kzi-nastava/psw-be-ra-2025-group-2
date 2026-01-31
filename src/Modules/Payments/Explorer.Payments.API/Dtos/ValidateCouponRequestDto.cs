using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.API.Dtos
{
    public class ValidateCouponRequestDto
    {
        public string Code { get; set; }
        public long TourId { get; set; }
    }
}
