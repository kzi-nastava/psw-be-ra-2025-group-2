
namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces
{
    public interface ITouristUnlockedFrameRepository
    {
        bool Exists(long userId, long frameId);
        IEnumerable<TouristUnlockedFrame> GetByUserId(long userId);
        void Add(TouristUnlockedFrame unlocked);
    }
}