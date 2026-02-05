using System;
using System.Collections.Generic;
using Explorer.BuildingBlocks.Core.Domain;
using System.Linq;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Club : Entity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public long OwnerId { get; init; }
        public List<string> ImageUrls { get; private set; }
        public ClubStatus Status { get; private set; }

        private readonly List<ClubMember> _members = new();
        public IReadOnlyCollection<ClubMember> Members => _members;

        private readonly List<ClubJoinRequest> _joinRequests = new();
        public IReadOnlyCollection<ClubJoinRequest> JoinRequests => _joinRequests;

        private readonly List<ClubInvitation> _invitations = new();
        public IReadOnlyCollection<ClubInvitation> Invitations => _invitations;
        private readonly List<ClubBadge> _badges = new();
        public IReadOnlyCollection<ClubBadge> Badges => _badges;
        public Club(string name, string description, long ownerId, List<string> imageUrls)
        {
            Name = name;
            Description = description;
            OwnerId = ownerId;
            ImageUrls = imageUrls ?? new List<string>();
            Status = ClubStatus.Active;
            Validate();
        }

        protected Club()
        {
            ImageUrls = new List<string>();
            Status = ClubStatus.Active;
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Invalid Name");

            if (string.IsNullOrWhiteSpace(Description))
                throw new ArgumentException("Invalid Description");

            if (ImageUrls == null || ImageUrls.Count == 0)
                throw new ArgumentException("At least one image is required");
        }

        public void Update(string name, string description, List<string> imageUrls)
        {
            Name = name;
            Description = description;
            ImageUrls = imageUrls ?? new List<string>();
            Validate();
        }

        public void Close() => Status = ClubStatus.Closed;
        public void Open() => Status = ClubStatus.Active;

        public void RequestMembership(long touristId)
        {
            if (Status != ClubStatus.Active)
                throw new InvalidOperationException("Club is not active.");

            if (_members.Any(m => m.TouristId == touristId))
                throw new InvalidOperationException("Tourist is already a member.");

            if (_joinRequests.Any(r => r.TouristId == touristId))
                throw new InvalidOperationException("Join request already exists.");

            _joinRequests.Add(new ClubJoinRequest(Id, touristId));
        }

        public void WithdrawRequest(long touristId)
        {
            var request = _joinRequests.FirstOrDefault(r => r.TouristId == touristId);
            if (request == null)
                throw new InvalidOperationException("Join request not found.");

            _joinRequests.Remove(request);
        }

        public void AcceptRequest(long touristId)
        {
            var request = _joinRequests.FirstOrDefault(r => r.TouristId == touristId);
            if (request == null)
                throw new InvalidOperationException("Join request not found.");

            _joinRequests.Remove(request);
            _members.Add(new ClubMember(Id, touristId));
        }

        public void RejectRequest(long touristId)
        {
            var request = _joinRequests.FirstOrDefault(r => r.TouristId == touristId);
            if (request == null)
                throw new InvalidOperationException("Join request not found.");

            _joinRequests.Remove(request);
        }

        public void InviteTourist(long touristId)
        {
            if (Status != ClubStatus.Active)
                throw new InvalidOperationException("Club is not active.");

            if (_members.Any(m => m.TouristId == touristId))
                throw new InvalidOperationException("Tourist is already a member.");

            if (_invitations.Any(i => i.TouristId == touristId))
                throw new InvalidOperationException("Invitation already exists.");

            if (_joinRequests.Any(r => r.TouristId == touristId))
                throw new InvalidOperationException("Tourist already has a pending join request.");

            _invitations.Add(new ClubInvitation(Id, touristId));
        }

        public void AcceptInvitation(long touristId)
        {
            var invitation = _invitations.FirstOrDefault(i => i.TouristId == touristId);
            if (invitation == null)
                throw new InvalidOperationException("Invitation not found.");

            _invitations.Remove(invitation);
            _members.Add(new ClubMember(Id, touristId));
        }

        public void RejectInvitation(long touristId)
        {
            var invitation = _invitations.FirstOrDefault(i => i.TouristId == touristId);
            if (invitation == null)
                throw new InvalidOperationException("Invitation not found.");

            _invitations.Remove(invitation);
        }

        public void RemoveMember(long touristId)
        {
            var member = _members.FirstOrDefault(m => m.TouristId == touristId);
            if (member == null)
                throw new InvalidOperationException("Member not found.");

            _members.Remove(member);
        }
        
        public List<ClubBadge> AwardMissingBadges(int totalXp, int stepXp = 500)
        {
            if (stepXp <= 0) throw new ArgumentException("Invalid stepXp.");

            var maxMilestone = (totalXp / stepXp) * stepXp;
            if (maxMilestone < stepXp) return new();

            var existing = _badges.Select(b => b.MilestoneXp).ToHashSet();
            var created = new List<ClubBadge>();

            for (int milestone = stepXp; milestone <= maxMilestone; milestone += stepXp)
            {
                if (existing.Contains(milestone)) continue;

                var badge = new ClubBadge(Id, milestone);
                _badges.Add(badge);
                created.Add(badge);
            }

            return created;
        }


    }
}
