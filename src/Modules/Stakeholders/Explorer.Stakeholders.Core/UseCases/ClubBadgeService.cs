using Explorer.Encounters.API.Internal;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Domain;
using  Explorer.Stakeholders.API.Public;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ClubBadgeService : IClubBadgeService
    {
        private readonly IClubRepository _clubRepository;
        private readonly IUserService _userService;
        private readonly IInternalTouristProgressService _progressService;

        public ClubBadgeService(
            IClubRepository clubRepository,
            IUserService userService,
            IInternalTouristProgressService progressService)
        {
            _clubRepository = clubRepository;
            _userService = userService;
            _progressService = progressService;
        }

        public List<int> GetClubBadges(long clubId, long requesterId)
        {
            var club = EnsureAllowed(clubId, requesterId);
            return club.Badges.Select(b => b.MilestoneXp).OrderBy(x => x).ToList();
        }

        public ClubBadgeAwardResultDto RecalculateAndAward(long clubId, long requesterId, int stepXp = 500)
        {
            var club = EnsureAllowed(clubId, requesterId);

            // members + owner (bez duplikata)
            var candidateIds = club.Members.Select(m => m.TouristId).Append(club.OwnerId).Distinct().ToList();

            // samo stvarni turisti (isti fazon kao u leaderboard servisu)
            var tourists = _userService.GetTourists(null)
                .Where(t => candidateIds.Contains(t.Id))
                .Select(t => t.Id)
                .ToHashSet();

            var memberIds = candidateIds.Where(id => tourists.Contains(id)).ToList();

            var xpRows = _progressService.GetXpForUsers(memberIds);
            var totalXp = xpRows.Sum(x => x.TotalXp);

            var before = club.Badges.Select(b => b.MilestoneXp).ToHashSet();
            var newBadges = club.AwardMissingBadges(totalXp, stepXp);

            if (newBadges.Count > 0)
            {
                _clubRepository.AddBadges(newBadges); // NOVA metoda
            }

            var after = club.Badges.Select(b => b.MilestoneXp).OrderBy(x => x).ToList();

            return new ClubBadgeAwardResultDto
            {
                ClubId = clubId,
                TotalXp = totalXp,
                AddedBadges = after.Count - before.Count,
                CurrentMilestones = after
            };
        }

        private Club EnsureAllowed(long clubId, long requesterId)
        {
            var club = _clubRepository.Get(clubId);
            if (club == null) throw new KeyNotFoundException($"Club {clubId} not found.");

            var isOwner = club.OwnerId == requesterId;
            var isMember = club.Members.Any(m => m.TouristId == requesterId);
            if (!isOwner && !isMember) throw new UnauthorizedAccessException("Not allowed.");

            return club;
        }
    }
}
