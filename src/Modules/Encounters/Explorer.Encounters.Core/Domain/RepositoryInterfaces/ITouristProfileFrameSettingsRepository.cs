namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces
{
    public interface ITouristProfileFrameSettingsRepository
    {
        TouristProfileFrameSettings? GetByUserId(long userId);
        TouristProfileFrameSettings Create(TouristProfileFrameSettings settings);
        void Update(TouristProfileFrameSettings settings);
    }
}
