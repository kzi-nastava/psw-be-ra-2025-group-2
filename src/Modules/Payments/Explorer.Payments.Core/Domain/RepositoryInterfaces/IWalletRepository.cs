using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.Core.Domain.Wallets;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface IWalletRepository
    {
        Wallet? GetByTouristId(long  touristId);
        Wallet Create(Wallet wallet);
        Wallet Update(Wallet wallet);
    }
}
