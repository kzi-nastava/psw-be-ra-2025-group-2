using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.API.Public
{
    public interface IWalletService
    {
        int GetBalance(long touristId);
        void AdminDeposit(long fromuristId, int amount);
    }
}
