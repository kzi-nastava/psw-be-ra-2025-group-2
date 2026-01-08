using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TourSaleDbRepository : ITourSaleRepository
    {
        private readonly ToursContext _db;

        public TourSaleDbRepository(ToursContext db)
        {
            _db = db;
        }

        public TourSale Create(TourSale sale)
        {
            _db.TourSales.Add(sale);
            _db.SaveChanges();
            return sale;
        }

        public void Delete(long id)
        {
            var entity = _db.TourSales.Find(id);
            if (entity == null) throw new KeyNotFoundException();
            _db.TourSales.Remove(entity);
            _db.SaveChanges();
        }

        public TourSale? GetById(long id) => _db.TourSales.Find(id);

        public long GetCount() => _db.TourSales.Count();

        public long GetCountByAuthor(long authorId)
            => _db.TourSales.Count(x => x.AuthorId == authorId);

        public long GetCountActive()
            => _db.TourSales.Count(x => x.StartDate <= DateTime.UtcNow && x.EndDate >= DateTime.UtcNow);

        public PagedResult<TourSale> GetPaged(int page, int pageSize)
            => _db.TourSales.GetPaged(page, pageSize).Result;

        public PagedResult<TourSale> GetPagedByAuthor(long authorId, int page, int pageSize)
            => _db.TourSales.Where(x => x.AuthorId == authorId).GetPaged(page, pageSize).Result;

        public PagedResult<TourSale> GetActive(int page, int pageSize)
            => _db.TourSales
                .Where(x => x.StartDate <= DateTime.UtcNow && x.EndDate >= DateTime.UtcNow)
                .GetPaged(page, pageSize).Result;

        public TourSale Update(TourSale sale)
        {
            _db.Update(sale);
            _db.SaveChanges();
            return sale;
        }
    }
}
