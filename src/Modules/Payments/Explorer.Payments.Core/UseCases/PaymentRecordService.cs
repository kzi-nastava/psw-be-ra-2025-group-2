using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Domain.ShoppingCarts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.UseCases
{
    public class PaymentRecordService : IPaymentRecordService
    {
        private readonly IPaymentRecordRepository _repository;
        private readonly IMapper _mapper;

        public PaymentRecordService(IPaymentRecordRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public PaymentRecordDto Create(PaymentRecordDto record)
        {
            var created = _repository.Create(_mapper.Map<PaymentRecord> (record));
            return _mapper.Map<PaymentRecordDto>(created);
        }
    }
}
