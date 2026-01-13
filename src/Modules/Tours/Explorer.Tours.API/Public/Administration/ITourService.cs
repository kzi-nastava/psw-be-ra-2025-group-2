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
        TourDto Get(long id);
        Task<TourDto?> GetByIdAsync(long id, long authorId);
        TourDto Update(long id, UpdateTourDto dto);
        void Delete(long id);
        Task<KeyPointDto> AddKeyPoint(long tourId, KeyPointDto dto);
        Task<KeyPointDto> UpdateKeyPoint(long tourId, int ordinalNo, KeyPointDto dto);
        void RemoveKeyPoint(long tourId, int ordinalNo);
        void Publish(long tourId, long authorId);
        void Archive(long id);
        void Reactivate(long id);
        PagedResult<TourDto> GetByRange(double lat, double lon, int range, int page, int pageSize);
        TourDto? GetPublishedTour(long tourId); // Vraća objavljenu turu bez provere autora

        List<TourEquipmentItemDto> GetEquipmentForTour(long tourId, long authorId);
        public List<TourEquipmentItemDto> GetAllEquipmentForAuthor(long authorId);
        void UpdateEquipmentForTour(long tourId, long authorId, List<long> equipmentIds);
        public PagedResultDto<PublishedTourPreviewDto> GetPublishedForTourist(int page, int pageSize);
        IEnumerable<PublishedTourPreviewDto> GetTrendingTours();

        /* Tourist's options */

        // TODO Promeniti kasnije
        IEnumerable<TourDto> GetAvailableForTourist(long touristId);

        public TourReviewDto AddReview(long tourId, long touristId, int rating, string comment, List<string> images);

        public TourReviewDto UpdateReview(TourReviewDto reviewDto);
        void DeleteReview(long touristId, long tourId);
    }

}
