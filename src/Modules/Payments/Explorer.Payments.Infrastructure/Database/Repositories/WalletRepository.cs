using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Domain.Wallets;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class WalletRepository :IWalletRepository
    {
        private readonly PaymentsContext _context;
        public WalletRepository(PaymentsContext context)
        {
            _context = context;
        }

        public Wallet? GetByTouristId(long touristId)
        {
            return _context.Wallets
                .FirstOrDefault(w => w.TouristId == touristId);
        }

        public Wallet Create(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            _context.SaveChanges();
            return wallet;
        }

        public Wallet Update(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            _context.SaveChanges();
            return wallet;
        }
    }
}
