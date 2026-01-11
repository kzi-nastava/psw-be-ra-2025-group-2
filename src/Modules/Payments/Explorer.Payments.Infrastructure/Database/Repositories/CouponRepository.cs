using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly PaymentsContext _context;

        public CouponRepository(PaymentsContext context)
        {
            _context = context;
        }

        public Coupon Create(Coupon coupon) 
        {
            _context.Coupons.Add(coupon);
            _context.SaveChanges();
            return coupon;
        }

        public Coupon Update(Coupon coupon)
        {
            _context.Coupons.Update(coupon);
            _context.SaveChanges();
            return coupon;
        }

        public void Delete(Coupon coupon) 
        {
            _context.Coupons.Remove(coupon);
            _context.SaveChanges();
        }

        public Coupon? GetByCode(string code) 
        {
            return _context.Coupons.FirstOrDefault(x => x.Code == code);
        }
        public List<Coupon> GetByAuthor(long authorId)
        {
            return _context.Coupons
                .Where(c => c.AuthorId == authorId)
                .ToList();
        }
    }
}
