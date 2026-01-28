using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Internal;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _clubRepository;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;

        public ClubService(
            IClubRepository clubRepository,
            IMapper mapper,
            INotificationService notificationService,
            IUserService userService)
        {
            _clubRepository = clubRepository;
            _mapper = mapper;
            _notificationService = notificationService;
            _userService = userService;
        }

        public ClubDto Get(long id)
        {
            var club = _clubRepository.Get(id);
            return club == null ? null : _mapper.Map<ClubDto>(club);
        }

        public ClubDto Create(ClubDto club)
        {
            var entity = _mapper.Map<Club>(club);
            var created = _clubRepository.Create(entity);
            return _mapper.Map<ClubDto>(created);
        }

        public ClubDto Update(ClubDto club)
        {
            var existing = _clubRepository.Get(club.Id);
            if (existing == null)
                throw new KeyNotFoundException($"Club {club.Id} not found.");

            existing.Update(club.Name, club.Description, club.ImageUrls);

            var updated = _clubRepository.Update(existing);
            return _mapper.Map<ClubDto>(updated);
        }

        public void Delete(long id)
        {
            _clubRepository.Delete(id);
        }

        public List<ClubDto> GetAll()
        {
            var clubs = _clubRepository.GetAll();
            return clubs.Select(_mapper.Map<ClubDto>).ToList();
        }

        public void InviteTourist(long clubId, long ownerId, long touristId)
        {
            var club = _clubRepository.Get(clubId);
            if (club == null)
                throw new KeyNotFoundException($"Club {clubId} not found.");

            if (club.OwnerId != ownerId)
                throw new System.UnauthorizedAccessException("Only the owner can invite tourists.");

            club.InviteTourist(touristId);
            _notificationService.SendInvitation(touristId, clubId);

            _clubRepository.Update(club);
        }

        public void AcceptInvitation(long clubId, long touristId)
        {
            var club = _clubRepository.Get(clubId);
            if (club == null)
                throw new KeyNotFoundException($"Club {clubId} not found.");

            club.AcceptInvitation(touristId);
            _clubRepository.Update(club);
        }

        public void RejectInvitation(long clubId, long touristId)
        {
            var club = _clubRepository.Get(clubId);
            if (club == null)
                throw new KeyNotFoundException($"Club {clubId} not found.");

            club.RejectInvitation(touristId);
            _clubRepository.Update(club);
        }

        public void RemoveMember(long clubId, long ownerId, long touristId)
        {
            var club = _clubRepository.Get(clubId);
            if (club == null)
                throw new KeyNotFoundException($"Club {clubId} not found.");

            if (club.OwnerId != ownerId)
                throw new System.UnauthorizedAccessException("Only the owner can remove members.");

            club.RemoveMember(touristId);
            _clubRepository.Update(club);
        }

        public void RequestMembership(long clubId, long touristId)
        {
            var club = _clubRepository.Get(clubId);
            if (club == null)
                throw new KeyNotFoundException($"Club {clubId} not found.");

            club.RequestMembership(touristId);
            _clubRepository.Update(club);
        }

        public void WithdrawMembershipRequest(long clubId, long touristId)
        {
            var club = _clubRepository.Get(clubId);
            if (club == null)
                throw new KeyNotFoundException($"Club {clubId} not found.");

            club.WithdrawRequest(touristId);
            _clubRepository.Update(club);
        }

        public void AcceptMembershipRequest(long clubId, long ownerId, long touristId)
        {
            var club = _clubRepository.Get(clubId);
            if (club == null)
                throw new KeyNotFoundException($"Club {clubId} not found.");

            if (club.OwnerId != ownerId)
                throw new System.UnauthorizedAccessException("Only the owner can accept membership requests.");

            club.AcceptRequest(touristId);
            _notificationService.SendMembershipAccepted(touristId, clubId);

            _clubRepository.Update(club);
        }

        public void RejectMembershipRequest(long clubId, long ownerId, long touristId)
        {
            var club = _clubRepository.Get(clubId);
            if (club == null)
                throw new KeyNotFoundException($"Club {clubId} not found.");

            if (club.OwnerId != ownerId)
                throw new System.UnauthorizedAccessException("Only the owner can reject membership requests.");

            club.RejectRequest(touristId);
            _notificationService.SendMembershipRejected(touristId, clubId);

            _clubRepository.Update(club);
        }

        public List<InvitableTouristDto> GetInvitableTourists(long clubId, long ownerId, string? query)
        {
            var club = _clubRepository.Get(clubId);
            if (club == null)
                throw new KeyNotFoundException($"Club {clubId} not found.");

            if (club.OwnerId != ownerId)
                throw new System.UnauthorizedAccessException("Only the owner can load invitable tourists.");

            var excluded = new HashSet<long> { ownerId };

            foreach (var m in club.Members) excluded.Add(m.TouristId);
            foreach (var i in club.Invitations) excluded.Add(i.TouristId);
            foreach (var r in club.JoinRequests) excluded.Add(r.TouristId);

            var tourists = _userService.GetTourists(query);

            return tourists
                .Where(t => !excluded.Contains(t.Id))
                .Select(t => new InvitableTouristDto
                {
                    Id = t.Id,
                    Username = t.Username,
                })
                .ToList();
        }

        public List<TouristBasicDto> GetJoinRequests(long clubId, long ownerId)
        {
            var club = _clubRepository.Get(clubId);
            if (club == null)
                throw new KeyNotFoundException($"Club {clubId} not found.");

            if (club.OwnerId != ownerId)
                throw new System.UnauthorizedAccessException("Only the owner can load join requests.");

            var ids = club.JoinRequests.Select(r => r.TouristId).ToHashSet();
            var tourists = _userService.GetTourists(null);

            return tourists
                .Where(t => ids.Contains(t.Id))
                .Select(t => new TouristBasicDto { Id = t.Id, Username = t.Username })
                .ToList();
        }

        public List<TouristBasicDto> GetMembers(long clubId, long ownerId)
        {
            var club = _clubRepository.Get(clubId);
            if (club == null)
                throw new KeyNotFoundException($"Club {clubId} not found.");

            if (club.OwnerId != ownerId)
                throw new System.UnauthorizedAccessException("Only the owner can load members.");

            var ids = club.Members.Select(m => m.TouristId).ToHashSet();
            var tourists = _userService.GetTourists(null);

            return tourists
                .Where(t => ids.Contains(t.Id))
                .Select(t => new TouristBasicDto { Id = t.Id, Username = t.Username })
                .ToList();
        }

        public List<long> GetMyInvitationClubIds(long touristId)
        {
            return _clubRepository.GetInvitationClubIds(touristId);
        }

        public List<long> GetMyMembershipClubIds(long touristId)
        {
            return _clubRepository.GetMemberClubIds(touristId);
        }

        public List<long> GetMyJoinRequestClubIds(long touristId)
        {
            return _clubRepository.GetMyJoinRequestClubIds(touristId);
        }
    }
}
