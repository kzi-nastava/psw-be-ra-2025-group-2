using AutoMapper;
using Explorer.Payments.API.Internal;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.UseCases.Internal
{
    public class InternalTokenService : IInternalTokenService
    {
        private readonly ITourPurchaseTokenRepository _tokenRepository;

        public InternalTokenService(ITourPurchaseTokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        public IEnumerable<long> GetPurchasedTourIds(long touristId)
        {
            return _tokenRepository.GetTourIdsByTouristId(touristId);
        }

        public IEnumerable<long> GetTouristIdsByTourId(long tourId)
        {
            return _tokenRepository.GetTouristIdsByTourId(tourId);
        }
    }
}
