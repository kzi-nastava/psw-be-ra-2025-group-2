using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Domain.ShoppingCarts;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.UseCases
{
    public class PaymentRecordService : IPaymentRecordService
    {
        private readonly IPaymentRecordRepository _paymentRecordRepository;
        private readonly IShoppingCartRepository _cartRepository;

        public PaymentRecordService(
            IPaymentRecordRepository paymentRecordRepository,
            IShoppingCartRepository cartRepository)
        {
            _paymentRecordRepository = paymentRecordRepository;
            _cartRepository = cartRepository;
        }

        public void Checkout(long touristId)
        {
            var cart = _cartRepository.GetByTouristId(touristId);
            if (cart == null)
                throw new KeyNotFoundException("Shopping cart not found.");

            if (!cart.Items.Any())
                throw new InvalidOperationException("Shopping cart is empty.");

            var now = DateTime.UtcNow;

            foreach (var item in cart.Items)
            {
                var record = new PaymentRecord(
                    touristId,
                    item.TourId,
                    item.Price.Amount,
                    now
                );

                _paymentRecordRepository.Create(record);
            }

            cart.ClearCart();
            _cartRepository.Update(cart);
        }
    }
}
