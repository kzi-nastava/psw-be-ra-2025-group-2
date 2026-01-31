using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Dtos.Encounter
{
    public class LevelRewardDto
    {
        public int Level { get; set; }
        public int DiscountPercentage { get; set; }
        public int CouponValidityDays { get; set; }
        public string Description { get; set; }
    }
}
