using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Encounters.API.Internal;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ClubLeaderboardService : IClubLeaderboardService
    {
        private readonly IClubRepository _clubRepository;
        private readonly IUserService _userService; // tvoj postojeći user/tourist servis iz stakeholders
        private readonly IInternalTouristProgressService _progressService;

        public ClubLeaderboardService(
            IClubRepository clubRepository,
            IUserService userService,
            IInternalTouristProgressService progressService)
        {
            _clubRepository = clubRepository;
            _userService = userService;
            _progressService = progressService;
        }

        public List<ClubLeaderboardRowDto> GetLeaderboard(long clubId, long requesterId)
        {
            var club = _clubRepository.Get(clubId);
            if (club == null) throw new KeyNotFoundException($"Club {clubId} not found.");

            var isOwner = club.OwnerId == requesterId;
            var isMember = club.Members.Any(m => m.TouristId == requesterId);
            if (!isOwner && !isMember) throw new UnauthorizedAccessException("Not allowed.");

            // ✅ 1) IDs: members + owner (bez duplikata)
            var candidateIds = club.Members.Select(m => m.TouristId).ToList();
            candidateIds.Add(club.OwnerId);
            candidateIds = candidateIds.Distinct().ToList();

            // ✅ 2) Uzmemo turiste samo među candidateIds
            // (owner će ući samo ako je stvarno turist)
            var tourists = _userService.GetTourists(null)
                .Where(t => candidateIds.Contains(t.Id))
                .ToDictionary(t => t.Id);

            // ✅ 3) Leaderboard pravimo samo od stvarnih turista
            var memberIds = candidateIds.Where(id => tourists.ContainsKey(id)).ToList();

            // ✅ 4) XP + level za sve njih
            var xpRows = _progressService.GetXpForUsers(memberIds);

            var sorted = xpRows
                .Select(x => new { x.UserId, x.TotalXp, x.Level })
                .OrderByDescending(x => x.TotalXp)
                .ThenBy(x => x.UserId)
                .ToList();

            var result = new List<ClubLeaderboardRowDto>();
            int rank = 0;
            int? prevXp = null;

            foreach (var row in sorted)
            {
                if (prevXp == null || row.TotalXp != prevXp)
                {
                    rank++;
                    prevXp = row.TotalXp;
                }

                tourists.TryGetValue(row.UserId, out var t);

                result.Add(new ClubLeaderboardRowDto
                {
                    Rank = rank,
                    TouristId = row.UserId,
                    Username = t?.Username ?? "Unknown",
                    Level = row.Level,
                    TotalXp = row.TotalXp
                });
            }

            return result;
        }

    }
}