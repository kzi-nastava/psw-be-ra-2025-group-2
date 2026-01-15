using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.API.Dtos
{
    public class WalletDto
    {
        public long TouristId { get; set; }
        public int Balance { get; set; }
    }
}
