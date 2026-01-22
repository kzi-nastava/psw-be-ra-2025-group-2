namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces
{
    public interface IProfileFrameRepository
    {
        ProfileFrame? GetByLevelRequirement(int levelRequirement);
        IEnumerable<ProfileFrame> GetAll();
        void Add(ProfileFrame frame);
    }
}