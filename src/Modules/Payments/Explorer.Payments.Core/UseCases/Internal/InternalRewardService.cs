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

        public string GrantCoupon(long userId, int discountPercentage, DateTime? validUntil, string description)
        {
            try
            {
                // pravimo globalni kupon
                var coupon = _couponService.CreateRewardCoupon(
                discountPercentage,
                tourId: null,
                authorId: null,
                validUntil: validUntil
                );

                var message = $"🎉 {description}\nYou received a {discountPercentage}% discount coupon!\nCode: {coupon.Code}\nValid until: {validUntil?.ToString("dd.MM.yyyy") ?? "no expiration"}";
                NotifyUser(userId, message);
                return coupon.Code;
            }
            catch (Exception ex)
            {
                // Ovde stavi Breakpoint da vidiš zašto Coupon ne prolazi
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void NotifyUser(long userId, string message)
        {
            _notificationService.NotifyUserMessage(userId, message);
        }
    }
}
