using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface ICouponRepository
    {
        Coupon Create(Coupon coupon);
        Coupon Update(Coupon coupon);
        void Delete(Coupon coupon);
        Coupon? GetByCode(string code);
        List<Coupon> GetByAuthor(long authorId);
    }
}
