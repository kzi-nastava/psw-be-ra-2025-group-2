using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain.Wallets
{
    public class WalletTransaction:Entity
    {
        public int Amount { get; private set; }
        public WalletTransactionType Type { get; private set; }
        public DateTime CreatedAt { get; private set; } 

        private WalletTransaction() { }
        public WalletTransaction(int amount, WalletTransactionType type)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be positive");
            }
            Amount = amount;
            Type = type;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
