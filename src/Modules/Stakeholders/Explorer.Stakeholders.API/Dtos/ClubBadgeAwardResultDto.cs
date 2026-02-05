public class ClubBadgeAwardResultDto
{
    public long ClubId { get; set; }
    public int TotalXp { get; set; }
    public int AddedBadges { get; set; }
    public List<int> CurrentMilestones { get; set; } = new();
}