using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Infrastructure.Database;


namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class PaymentDbRepository : IPaymentRepository
    {
        private readonly PaymentsContext _dbContext; 

        public PaymentDbRepository(PaymentsContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
