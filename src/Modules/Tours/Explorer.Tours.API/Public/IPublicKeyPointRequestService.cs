using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public
{
    public interface IPublicKeyPointRequestService
    {
        IEnumerable<PublicKeyPointRequestDto> GetPendingRequests();

        void ApproveRequest(long requestId);

        void DenyRequest(long requestId, string comment);
    }
}
