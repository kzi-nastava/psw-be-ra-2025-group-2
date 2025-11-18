using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface ITourPreferencesRepository
    {
        TourPreferences GetByTouristId(long touristId);

        TourPreferences Create(TourPreferences entity);

        TourPreferences Update(TourPreferences entity);

        void Delete(long id);
    }
}
