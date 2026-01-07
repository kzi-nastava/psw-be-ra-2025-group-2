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

     
    }
}
