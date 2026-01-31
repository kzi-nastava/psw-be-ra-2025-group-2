using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.API.Dtos;


namespace Explorer.Payments.API.Public
{
    public interface ICouponService
    {
        CouponDto Create(CouponCreateDto dto, long authorId);
        CouponDto Update(string code, CouponCreateDto dto, long authorId);
        void Delete(string code, long authorId);
        CouponDto GetByCode(string code);
        List<CouponDto> GetByAuthor(long authorId, long? tourId);
        CouponDto CreateRewardCoupon(int discountPercentage, long? tourId, long? authorId, DateTime? validUntil); 
    }
}
