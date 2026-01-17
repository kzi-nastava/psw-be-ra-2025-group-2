using Explorer.Payments.API.Internal;
using Explorer.Stakeholders.Core.Domain.Exceptions;
using Explorer.Stakeholders.Core.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain
{
    public class TourOwnershipChecker : ITourOwnershipChecker
    {
        private readonly IInternalTokenService _tokenService;

        public TourOwnershipChecker(IInternalTokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public void CheckOwnership(long touristId, long tourId)
        {
            var tokens = _tokenService.GetPurchasedTourIds(touristId);

            if (!tokens.Contains(tourId))
                throw new TourNotOwnedException($"Tourist does not own the tour with id {tourId}.");
        }
    }
}
