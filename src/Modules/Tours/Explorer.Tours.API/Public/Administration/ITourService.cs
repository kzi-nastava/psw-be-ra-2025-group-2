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
        TourDto Update(long id, UpdateTourDto dto);
        void Delete(long id);
    }
}
