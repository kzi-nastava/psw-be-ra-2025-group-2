using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Exceptions;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TourReportDbRepository : ITourReportRepository
    {
        private readonly ToursContext _context;

        public TourReportDbRepository(ToursContext context)
        {
            _context = context;
        }

        public TourReport Create(TourReport report)
        {
            _context.TourReports.Add(report);
            _context.SaveChanges();
            return report;
        }

        public void Delete(long id)
        {
            var report = _context.TourReports.Find(id);
            if (report != null)
            {
                _context.TourReports.Remove(report);
                _context.SaveChanges();
            }
            else
            {
                throw new NotFoundException($"Not found: {id}.");
            }
        }

        public TourReport GetById(long id)
        {
            return _context.TourReports.Find(id);
        }

        public IEnumerable<TourReport> GetPending()
        {
            return _context.TourReports
                .Where(r => r.State == TourReportState.Pending)
                .ToList();
        }

        public TourReport Update(TourReport report)
        {
            try
            {
                _context.TourReports.Update(report);
                _context.SaveChanges();
            }
            catch(DbUpdateException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            return report;
        }

        public IEnumerable<TourReport> GetByTouristAndTour(long touristId, long tourId)
        {
            return _context.TourReports
                .Where(r => r.TouristId == touristId && r.TourId == tourId)
                .ToList();
        }
    }
}
