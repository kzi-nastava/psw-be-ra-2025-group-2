using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.Core.Domain.Wallets;
using Shouldly;

namespace Explorer.Payments.Tests.Unit
{
    public class WalletUnitTests
    {
        [Fact]
        public void Creates_with_zero_balance()
        {
            var wallet = new Wallet(-21);

            wallet.TouristId.ShouldBe(-21);
            wallet.Balance.ShouldBe(0);
        }

        [Fact]
        public void Creation_fails_for_invalid_tourist_id()
        {
            Should.Throw<ArgumentException>(() => new Wallet(0));
        }

        [Fact]
        public void Adds_adventure_coins()
        {
            var wallet = new Wallet(-21);

            wallet.AddAdventureCoins(100);

            wallet.Balance.ShouldBe(100);
        }

        [Fact]
        public void Adds_multiple_times()
        {
            var wallet = new Wallet(-21);

            wallet.AddAdventureCoins(50);
            wallet.AddAdventureCoins(30);
            wallet.AddAdventureCoins(20);

            wallet.Balance.ShouldBe(100);
        }

        [Fact]
        public void Add_fails_for_zero_amount()
        {
            var wallet = new Wallet(-21);

            Should.Throw<ArgumentException>(() => wallet.AddAdventureCoins(0));
        }

        [Fact]
        public void Add_fails_for_negative_amount()
        {
            var wallet = new Wallet(-21);

            Should.Throw<ArgumentException>(() => wallet.AddAdventureCoins(-50));
        }

        [Fact]
        public void Balance_accumulates_correctly()
        {
            var wallet = new Wallet(-21);

            wallet.AddAdventureCoins(100);
            wallet.Balance.ShouldBe(100);

            wallet.AddAdventureCoins(150);
            wallet.Balance.ShouldBe(250);

            wallet.AddAdventureCoins(50);
            wallet.Balance.ShouldBe(300);
        }

        [Fact]
        public void Different_wallets_are_independent()
        {
            var wallet1 = new Wallet(-21);
            var wallet2 = new Wallet(-22);

            wallet1.AddAdventureCoins(100);
            wallet2.AddAdventureCoins(200);

            wallet1.Balance.ShouldBe(100);
            wallet2.Balance.ShouldBe(200);
        }

        [Fact]
        public void Supports_large_amounts()
        {
            var wallet = new Wallet(-21);

            wallet.AddAdventureCoins(1000000);

            wallet.Balance.ShouldBe(1000000);
        }
    }
}
