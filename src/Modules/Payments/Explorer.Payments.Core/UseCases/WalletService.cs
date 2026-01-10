using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Domain.Wallets;

namespace Explorer.Payments.Core.UseCases
{
    public class WalletService :IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly INotificationService _notificationService;

        public WalletService(IWalletRepository walletRepository,  INotificationService notificationService)
        {
            _walletRepository = walletRepository;
            _notificationService = notificationService;
        }

        public int GetBalance(long touristId)
        {
            var wallet = _walletRepository.GetByTouristId(touristId);

            if(wallet == null) {
                wallet = new Wallet(touristId);
                _walletRepository.Create(wallet);
            }
            return wallet.Balance;
        }

        public void AdminDeposit(long touristId, int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be positive");
            }
            var wallet = _walletRepository.GetByTouristId(touristId);

            if (wallet == null)
            {
                wallet = new Wallet(touristId);
                _walletRepository.Create(wallet);
            }

            wallet.AddAdventureCoins(amount);
            _walletRepository.Update(wallet);

            var transaction = new WalletTransaction(amount, WalletTransactionType.AdminDeposit);

            _notificationService.NotifyWalletDeposit(touristId, amount);
        }
            
    }
}
