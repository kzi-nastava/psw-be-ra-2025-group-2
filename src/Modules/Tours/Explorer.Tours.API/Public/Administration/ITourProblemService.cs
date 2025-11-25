using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Administration
{
    public interface ITourProblemService
    {
        // turista prijavljuje problem
        TourProblemDto Create(CreateTourProblemDto dto);
        // svi problemi koje je prijavio
        List<TourProblemDto> GetForCreator();

        TourProblemDto Update(TourProblemDto dto);

        void Delete(long id);
    }
}
