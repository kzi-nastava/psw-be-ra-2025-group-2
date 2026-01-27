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
using Explorer.Tours.API.Internal;

namespace Explorer.Payments.Core.UseCases
{
    public class PaymentRecordService : IPaymentRecordService
    {
        private readonly IPaymentRecordRepository _paymentRecordRepository;
        private readonly IShoppingCartRepository _cartRepository;
        private readonly ITourStatisticsService _tourStats;

        public PaymentRecordService(
            IPaymentRecordRepository paymentRecordRepository,
            IShoppingCartRepository cartRepository,
            ITourStatisticsService tourStats)
        {
            _paymentRecordRepository = paymentRecordRepository;
            _cartRepository = cartRepository;
            _tourStats = tourStats;
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
                PaymentRecord record;
                if (item.ItemType == "TOUR")
                { 
                    record = new PaymentRecord(
                    touristId,
                    item.TourId.Value,
                    item.Price.Amount,
                    now
                    );
                }
                else
                {
                    record = new PaymentRecord(
                    touristId,
                    item.Price.Amount,
                    now,
                    item.BundleId.Value
                    );
                }


                _paymentRecordRepository.Create(record);
                if (item.ItemType == "TOUR")
                {
                    _tourStats.IncrementPurchases(item.TourId!.Value);
                }
                else
                {
                    if (item.TourIds != null && item.TourIds.Count > 0)
                    {
                        foreach (var tid in item.TourIds)
                            _tourStats.IncrementPurchases(tid);
                    }
                }
            }

            cart.ClearCart();
            _cartRepository.Update(cart);
        }

        public List<PaymentRecordDto> GetMine(long touristId)
        {
            var records = _paymentRecordRepository.GetByTouristId(touristId);

            return records.Select(r => new PaymentRecordDto
            {
                Id = r.Id,
                TouristId = r.TouristId,

                TourId = r.TourId,       
                BundleId = r.BundleId,  

                Price = r.Price,
                CreatedAt = r.CreatedAt
            }).ToList();
        }


        public PaymentRecordDto GetMineById(long touristId, long id)
        {
            var record = _paymentRecordRepository.GetByIdAndTouristId(id, touristId);
            if (record == null) throw new KeyNotFoundException("Payment record not found.");

            return new PaymentRecordDto
            {
                Id = record.Id,
                TouristId = record.TouristId,
                TourId = record.TourId,
                Price = record.Price,
                CreatedAt = record.CreatedAt
            };
        }

    }
}
