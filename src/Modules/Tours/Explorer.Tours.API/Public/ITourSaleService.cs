using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public
{
    public interface ITourSaleService
    {
        /*PagedResult<TourSaleDto> GetPagedByAuthor(long authorId, int page, int pageSize);
        PagedResult<TourSaleDto> GetActive(int page, int pageSize);*/

        TourSaleDto Create(TourSaleDto dto);
        TourSaleDto Update(long authorId, TourSaleDto dto);
        void Delete(long authorId, long id);

        int GetPageCount(int pageSize);
        int GetPageCountByAuthor(long authorId, int pageSize);
    }
}
