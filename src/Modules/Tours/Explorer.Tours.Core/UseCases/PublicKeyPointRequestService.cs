using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.UseCases
{
    public class PublicKeyPointRequestService : IPublicKeyPointRequestService
    {
        private readonly IPublicKeyPointRequestRepository _requestRepo;
        private readonly INotificationService _notificationService;

        public PublicKeyPointRequestService(
            IPublicKeyPointRequestRepository requestRepo,
            INotificationService notificationService)
        {
            _requestRepo = requestRepo;
            _notificationService = notificationService;
        }

        public IEnumerable<PublicKeyPointRequestDto> GetPendingRequests()
        {
            return _requestRepo
                .GetPending()
                .Select(r => new PublicKeyPointRequestDto
                {
                    Id = r.Id,
                    PublicKeyPointId = r.PublicKeyPointId,
                    AuthorId = r.AuthorId,
                    Status = r.Status.ToString(),
                    RejectionReason = r.RejectionReason,
                    CreatedAt = r.CreatedAt
                });
        }

        public void ApproveRequest(long requestId)
        {
            var request = _requestRepo.Get(requestId);
            if (request == null) throw new Exception("Request not found.");

            var keyPoint = _requestRepo.Get(request.PublicKeyPointId);
            if (keyPoint == null) throw new Exception("KeyPoint not found.");

            //request.Status = RequestStatus.Approved;
            //keyPoint.Status = KeyPointStatus.Public;

            _requestRepo.Update(request);
            _requestRepo.Update(keyPoint);

            _notificationService.NotifyAuthorApprovedAsync(
                request.AuthorId,
                "Vaš zahtev za javnu objavu ključne tačke je prihvaćen."
            );
        }

        public void DenyRequest(long requestId, string comment)
        {
            var request = _requestRepo.Get(requestId);
            if (request == null) throw new Exception("Request not found.");

            var keyPoint = _requestRepo.Get(request.PublicKeyPointId);
            if (keyPoint == null) throw new Exception("KeyPoint not found.");

           // request.Status = RequestStatus.Denied;
           // request.RejectionReason = comment;
            //keyPoint.Status = KeyPointStatus.Private;

            _requestRepo.Update(request);
            _requestRepo.Update(keyPoint);

            _notificationService.NotifyAuthorRejectedAsync(
                request.AuthorId,
                "Vaš zahtev za javnu objavu ključne tačke je odbijen. Razlog: ",
                comment 
            );
        }
    }
}
