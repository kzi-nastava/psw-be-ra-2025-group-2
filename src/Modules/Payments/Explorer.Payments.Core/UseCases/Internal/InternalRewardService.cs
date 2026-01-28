using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Encounters.API.Internal;
using Explorer.Payments.API.Public;
using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.Core.UseCases.Internal
{
    public class InternalRewardService : IInternalRewardService
    {
        private readonly ICouponService _couponService;
        private readonly INotificationService _notificationService;

        public InternalRewardService(ICouponService couponService, INotificationService notificationService)
        {
            _couponService = couponService;
            _notificationService = notificationService;
        }

        public void GrantCoupon(long userId, int discountPercentage, DateTime? validUntil, string description)
        {
            var couponDto = new CouponCreateDto
            {
                DiscountPercentage = discountPercentage,
                TourId = null, 
                ValidUntil = validUntil
            };

            var coupon = _couponService.Create(couponDto, userId);

            var message = $"🎉 {description}\nYou received a {discountPercentage}% discount coupon!\nCode: {coupon.Code}\nValid until: {validUntil?.ToString("dd.MM.yyyy") ?? "no expiration"}";
            NotifyUser(userId, message);
        }

        public void NotifyUser(long userId, string message)
        {
            _notificationService.NotifyUserMessage(userId, message);
        }
    }
}
