using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourSaleRepository
    {
        TourSale? GetById(long id);
        PagedResult<TourSale> GetPaged(int page, int pageSize);
        PagedResult<TourSale> GetPagedByAuthor(long authorId, int page, int pageSize);
        PagedResult<TourSale> GetActive(int page, int pageSize);

        TourSale Create(TourSale sale);
        TourSale Update(TourSale sale);
        void Delete(long id);

        long GetCount();
        long GetCountByAuthor(long authorId);
        long GetCountActive();
    }
}
