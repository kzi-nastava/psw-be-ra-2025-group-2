using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface ITourPreferencesService
    {
        TourPreferencesDto GetByTourist(long touristId);

        TourPreferencesDto Create(TourPreferencesDto dto);

        TourPreferencesDto Update(TourPreferencesDto dto);
        void Delete(long id);
    }
}
