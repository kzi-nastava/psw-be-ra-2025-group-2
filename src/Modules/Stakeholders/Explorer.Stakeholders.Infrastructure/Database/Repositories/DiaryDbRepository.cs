using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

    public class DiaryDbRepository : IDiaryRepository
{
    private readonly StakeholdersContext _context;

    public DiaryDbRepository(StakeholdersContext context)
    {
        _context = context;
    }

    public Diary Get(long id) => _context.Diaries.Find(id);

    public List<Diary> GetByUserId(long userId) =>
        _context.Diaries.Where(d => d.UserId == userId).ToList();

    public Diary Create(Diary diary)
    {
        _context.Diaries.Add(diary);
        _context.SaveChanges();
        return diary;
    }

    public Diary Update(Diary diary)
    {
        _context.Diaries.Update(diary);
        _context.SaveChanges();
        return diary;
    }

    public void Delete(long id)
    {
        var diary = _context.Diaries.Find(id);
        if (diary != null)
        {
            _context.Diaries.Remove(diary);
            _context.SaveChanges();
        }
    }
}
