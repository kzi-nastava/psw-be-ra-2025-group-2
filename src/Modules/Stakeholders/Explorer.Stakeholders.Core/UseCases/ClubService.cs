using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _clubRepository;
        private readonly IMapper _mapper;

        public ClubService(IClubRepository clubRepository, IMapper mapper)
        {
            _clubRepository = clubRepository;
            _mapper = mapper;
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
            var entity = _mapper.Map<Club>(club);
            var updated = _clubRepository.Update(entity);
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

            _clubRepository.Update(club);
        }
    }
}
