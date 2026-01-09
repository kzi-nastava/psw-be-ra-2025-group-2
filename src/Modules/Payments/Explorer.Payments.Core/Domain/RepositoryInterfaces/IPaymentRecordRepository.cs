using Explorer.Payments.Core.Domain.ShoppingCarts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface IPaymentRecordRepository
    {
        PaymentRecord Create(PaymentRecord record);
        List<PaymentRecord> GetByTouristId(long touristId);
        PaymentRecord? GetByIdAndTouristId(long id, long touristId);
    }
}
