using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourRepository
    {
        Task<Tour> AddAsync(Tour tour);
        Task<Tour?> GetByIdAsync(long id);
        IEnumerable<Tour> GetByIds(IEnumerable<long> ids);
        Task<IEnumerable<Tour>> GetByAuthorAsync(long authorId);
        Task UpdateAsync(Tour tour);
        Task DeleteAsync(Tour tour);

        public List<Tour> GetAllPublished();
        public List<Tour> GetAllNonDrafts();

        List<Tour> GetByIds(List<long> ids); //nova metoda za bundles


        List<Tour> GetAllPublished(int page, int pageSize); 
        Task<Tour?> GetTourWithKeyPointsAsync(long tourId);
        Task<Tour?> GetTourByKeyPointIdAsync(long keyPointId);
        Task<IEnumerable<Tour?>> GetAllAsync();
    }
}