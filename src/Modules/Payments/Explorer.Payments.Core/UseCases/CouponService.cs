using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _repository;
        private readonly IMapper _mapper;

        public CouponService(ICouponRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public CouponDto Create(CouponCreateDto dto, long authorId)
        {
            var coupon = new Coupon(dto.DiscountPercentage, authorId, dto.TourId, dto.ValidUntil);

            var saved = _repository.Create(coupon);
            return _mapper.Map<CouponDto>(saved);
        }

        public CouponDto Update(string code, CouponCreateDto dto, long authorId)
        {
            var coupon = _repository.GetByCode(code) ?? throw new KeyNotFoundException("Coupon not found");

            if(coupon.AuthorId != authorId)
            {
                throw new UnauthorizedAccessException();
            }

            coupon.Update(dto.DiscountPercentage, dto.TourId, dto.ValidUntil);

            var updated = _repository.Update(coupon);
            return _mapper.Map<CouponDto>(updated);
        }
        public void Delete(string code, long authorId)
        {
            var coupon = _repository.GetByCode(code) ?? throw new KeyNotFoundException("Coupon not found");

            if (coupon.AuthorId != authorId)
            {
                throw new UnauthorizedAccessException();
            }

            _repository.Delete(coupon);
        }

        public CouponDto GetByCode(string code)
        {
            var coupon = _repository.GetByCode(code);
            if (coupon == null)
            {
                throw new KeyNotFoundException("Coupon not found");
            }
            return _mapper.Map<CouponDto>(coupon);
        }

        public List<CouponDto> GetByAuthor(long authorId, long? tourId)
        {
            var coupons = _repository.GetByAuthor(authorId);

            if (tourId.HasValue)
            {
                coupons = coupons
                    .Where(c => c.TourId == tourId.Value)
                    .ToList();
            }

            return _mapper.Map<List<CouponDto>>(coupons);
        }

    }
}
