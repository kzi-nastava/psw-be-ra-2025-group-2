using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain.Wallets
{
    public class Wallet : AggregateRoot
    {
        public long TouristId { get; private set; }
        public int Balance { get; private set; }
        private Wallet() { }

        public Wallet(long touristId)
        {
            if (touristId == 0)
            {
                throw new ArgumentException("Invalid tourist id");
            }
            TouristId = touristId;
            Balance = 0;
        }

        public void AddAdventureCoins(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be positive");
            }
            Balance += amount;
        }
    }
}
