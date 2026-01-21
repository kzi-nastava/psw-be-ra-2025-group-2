using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.Core.Domain.Planner;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class PlannerDbRepository : IPlannerRepository
    {
        private readonly StakeholdersContext _context;

        public PlannerDbRepository(StakeholdersContext context)
        {
            _context = context;
        }

        public DayEntry? GetById(long id)
        {
            return _context.PlannerDayEntries.Include(x => x.Entries).ToList().FirstOrDefault(e => e.Id == id);
        }

        public DayEntry? GetByDate(long touristId, DateOnly date)
        {
            return _context.PlannerDayEntries
                .Include(x => x.Entries)
                .ToList()
                .FirstOrDefault(x => x.TouristId == touristId && x.Date == date);
        }

        public IEnumerable<DayEntry> GetByMonth(long touristId, int month, int year)
        {
            return _context.PlannerDayEntries
                .Include(x => x.Entries)
                .Where(x => x.TouristId == touristId &&
                            x.Date.Month == month &&
                            x.Date.Year == year)
                .OrderBy(x => x.Date)
                .ToList();
        }

        public DayEntry? GetByScheduleEntryId(long scheduleEntryId)
        {
            return _context.PlannerDayEntries
                .Include(x => x.Entries)
                .ToList()
                .FirstOrDefault(x => x.Entries.Any(e => e.Id == scheduleEntryId));
        }

        public DayEntry Create(DayEntry entry)
        {
            _context.PlannerDayEntries.Add(entry);
            _context.SaveChanges();
            return entry;
        }

        public DayEntry Update(DayEntry entry)
        {
            try
            {
                _context.PlannerDayEntries.Update(entry);
                _context.SaveChanges();
                return entry;
            }
            catch (DbUpdateException)
            {
                throw new NotFoundException($"Not found: {entry.Id}");
            }
        }

        public void Delete(long id)
        {
            var entry = _context.PlannerDayEntries.Find(id);
            if (entry == null) throw new NotFoundException($"Not found: {id}");

            _context.PlannerDayEntries.Remove(entry);
            _context.SaveChanges();
        }
    }
}
