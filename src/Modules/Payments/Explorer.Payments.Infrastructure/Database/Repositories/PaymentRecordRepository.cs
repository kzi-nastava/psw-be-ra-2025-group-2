using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Domain.ShoppingCarts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class PaymentRecordRepository : IPaymentRecordRepository
    {
        private readonly PaymentsContext _context;
        public PaymentRecordRepository(PaymentsContext context)
        {
            _context = context;
        }
        public PaymentRecord Create(PaymentRecord record)
        {
            _context.PaymentRecords.Add(record);
            _context.SaveChanges(); 
            return record;
        }

        public List<PaymentRecord> GetByTouristId(long touristId)
        {
            return _context.PaymentRecords
                .Where(r => r.TouristId == touristId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }

        public PaymentRecord? GetByIdAndTouristId(long id, long touristId)
        {
            return _context.PaymentRecords
                .FirstOrDefault(r => r.Id == id && r.TouristId == touristId);
        }

    }
}
