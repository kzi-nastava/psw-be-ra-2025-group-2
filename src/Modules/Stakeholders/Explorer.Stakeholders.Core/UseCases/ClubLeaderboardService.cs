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
        private readonly IUserService _userService; 
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

            var candidateIds = club.Members.Select(m => m.TouristId).ToList();
            candidateIds.Add(club.OwnerId);
            candidateIds = candidateIds.Distinct().ToList();

            var tourists = _userService.GetTourists(null)
                .Where(t => candidateIds.Contains(t.Id))
                .ToDictionary(t => t.Id);

            var memberIds = candidateIds.Where(id => tourists.ContainsKey(id)).ToList();

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

        public List<ClubLeaderboardClubRowDto> GetClubsLeaderboard(long requesterId)
        {

            var clubs = _clubRepository.GetAll(); 
            if (clubs == null || clubs.Count == 0) return new List<ClubLeaderboardClubRowDto>();

            var allCandidateIds = clubs
                .SelectMany(c => c.Members.Select(m => m.TouristId).Append(c.OwnerId))
                .Distinct()
                .ToList();

            var tourists = _userService.GetTourists(null)
                .Where(t => allCandidateIds.Contains(t.Id))
                .ToDictionary(t => t.Id);

            var touristIds = tourists.Keys.ToList();

            var xpRows = _progressService.GetXpForUsers(touristIds);

            var xpByUserId = xpRows.ToDictionary(x => x.UserId, x => x.TotalXp);

            var clubTotals = clubs.Select(c =>
            {
                var memberIds = c.Members.Select(m => m.TouristId).Append(c.OwnerId).Distinct();

                var touristMemberIds = memberIds.Where(id => tourists.ContainsKey(id)).ToList();

                var totalXp = touristMemberIds.Sum(id => xpByUserId.TryGetValue(id, out var xp) ? xp : 0);

                return new
                {
                    ClubId = c.Id,
                    ClubName = c.Name, 
                    MembersCount = touristMemberIds.Count,
                    TotalXp = totalXp
                };
            })

            .OrderByDescending(x => x.TotalXp)
            .ThenBy(x => x.ClubId)
            .ToList();

            var result = new List<ClubLeaderboardClubRowDto>();
            int rank = 0;
            int? prevXp = null;

            foreach (var c in clubTotals)
            {
                if (prevXp == null || c.TotalXp != prevXp)
                {
                    rank++;
                    prevXp = c.TotalXp;
                }

                result.Add(new ClubLeaderboardClubRowDto
                {
                    Rank = rank,
                    ClubId = c.ClubId,
                    ClubName = c.ClubName ?? "",
                    MembersCount = c.MembersCount,
                    TotalXp = c.TotalXp
                });
            }

            return result;
        }

    }
}