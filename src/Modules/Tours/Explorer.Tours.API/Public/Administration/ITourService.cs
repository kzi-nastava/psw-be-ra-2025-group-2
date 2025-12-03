using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Dtos;

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
    }
}
