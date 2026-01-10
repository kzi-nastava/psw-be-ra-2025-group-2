using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.Core.Domain;

namespace Explorer.Payments.Core.Mappers
{
    public class CouponProfile : Profile
    {
        public CouponProfile()
        {
            CreateMap<Coupon, CouponDto>().ReverseMap();
        }
    }
}
