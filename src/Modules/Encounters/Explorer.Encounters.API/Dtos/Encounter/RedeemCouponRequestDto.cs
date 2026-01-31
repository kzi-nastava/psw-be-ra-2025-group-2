using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Dtos.Encounter
{
    public class RedeemCouponRequestDto
    {
        public long UserId { get; set; }
        public string CouponCode { get; set; }
        public long? TourId { get; set; }
    }
}
