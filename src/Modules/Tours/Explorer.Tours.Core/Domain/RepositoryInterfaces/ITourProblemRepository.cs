using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourProblemRepository
    {
        TourProblem Create(TourProblem newProblem);
        TourProblem Get(long id);
        List<TourProblem> GetForCreator();
        TourProblem Update(TourProblem Problem);

        void Delete(long id);
    }
}
