using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Dtos.Encounter
{
    public class UserRewardDto
    {
        public long UserId { get; set; }
        public int Level { get; set; }
        public string CouponCode { get; set; }
        public int DiscountPercentage { get; set; }
        public DateTime GrantedAt { get; set; }
        public DateTime? ValidUntil { get; set; }
        public bool IsUsed { get; set; }
        public string Description { get; set; }
    }
}
