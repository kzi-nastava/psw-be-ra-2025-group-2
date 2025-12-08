using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public.Administration
{
    public interface ITourService
    {
        TourDto Create(CreateTourDto dto);
        IEnumerable<TourDto> GetByAuthor(long authorId);
        TourDto? GetById(long id, long authorId);
        TourDto Update(long id, UpdateTourDto dto);
        void Delete(long id);
        void AddKeyPoint(long tourId, KeyPointDto dto);
        void UpdateKeyPoint(long tourId, int ordinalNo, KeyPointDto dto);
        void RemoveKeyPoint(long tourId, int ordinalNo);
        PagedResult<TourDto> GetByRange(double lat, double lon, int range, int page, int pageSize);
        TourDto? GetPublishedTour(long tourId); 

    }
}
