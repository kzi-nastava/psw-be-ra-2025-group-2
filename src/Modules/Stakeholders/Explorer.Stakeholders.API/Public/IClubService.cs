using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IClubService
    {
        ClubDto Create(ClubDto club);
        ClubDto Update(ClubDto club);
        void Delete(long id);
        List<ClubDto> GetAll();
        ClubDto Get(long id);

        void InviteTourist(long clubId, long ownerId, long touristId);
        void AcceptInvitation(long clubId, long touristId);
        void RejectInvitation(long clubId, long touristId);
        void RemoveMember(long clubId, long ownerId, long touristId);

        void RequestMembership(long clubId, long touristId);
        void WithdrawMembershipRequest(long clubId, long touristId);
        void AcceptMembershipRequest(long clubId, long ownerId, long touristId);
        void RejectMembershipRequest(long clubId, long ownerId, long touristId);
    }
}
