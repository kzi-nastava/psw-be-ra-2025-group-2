public interface IClubBadgeService
{
    ClubBadgeAwardResultDto RecalculateAndAward(long clubId, long requesterId, int stepXp = 500);
    List<int> GetClubBadges(long clubId, long requesterId);
}