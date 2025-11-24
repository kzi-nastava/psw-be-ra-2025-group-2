
namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IDiaryRepository
    {
        Diary Create(Diary diary);
        Diary Update(Diary diary);
        void Delete(long id);
        List<Diary> GetByUserId(long userId);
        Diary Get(long id);
    }
}
