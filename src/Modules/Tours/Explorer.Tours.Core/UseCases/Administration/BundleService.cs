using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Public;

namespace Explorer.Tours.Core.UseCases.Administration
{
    public class BundleService : IBundleService
    {
        private readonly IBundleRepository _bundleRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public BundleService(IBundleRepository bundleRepository, ITourRepository tourRepository, IMapper mapper)
        {
            _bundleRepository = bundleRepository;
            _tourRepository = tourRepository;
            _mapper = mapper;
        }

        public BundleDto Create(long authorId, CreateBundleDto dto)
        {
            // Validacija da ture postoje
            var tours = _tourRepository.GetByIds(dto.TourIds);

            if (tours.Count != dto.TourIds.Count)
                throw new KeyNotFoundException("Neke od izabranih tura ne postoje.");  // ✅ ISPRAVNO



            // Validacija da sve ture pripadaju autoru
            if (tours.Any(t => t.AuthorId != authorId))
                throw new UnauthorizedAccessException("Možeš dodati samo svoje ture u bundle.");

            // Kreiraj bundle
            var bundle = new Bundle(dto.Name, dto.Price, authorId, dto.TourIds);

            var created = _bundleRepository.Create(bundle);

            return MapToDto(created, tours);
        }

        public BundleDto GetById(long id)
        {
            var bundle = _bundleRepository.GetById(id);
            if (bundle == null)
                throw new KeyNotFoundException($"Bundle sa ID {id} nije pronađen.");

            var tours = _tourRepository.GetByIds(bundle.TourIds);
            return MapToDto(bundle, tours);
        }

        public List<BundleDto> GetByAuthorId(long authorId)
        {
            var bundles = _bundleRepository.GetByAuthorId(authorId);
            var result = new List<BundleDto>();

            foreach (var bundle in bundles)
            {
                var tours = _tourRepository.GetByIds(bundle.TourIds);
                result.Add(MapToDto(bundle, tours));
            }

            return result;
        }

        public BundleDto Update(long authorId, long bundleId, UpdateBundleDto dto)
        {
            var bundle = _bundleRepository.GetById(bundleId);
            if (bundle == null)
                throw new KeyNotFoundException("Bundle nije pronađen.");

            if (bundle.AuthorId != authorId)
                throw new UnauthorizedAccessException("Nemaš dozvolu da menjaš ovaj bundle.");

            // Validacija da ture postoje
            var tours = _tourRepository.GetByIds(dto.TourIds);

            if (tours.Count != dto.TourIds.Count)
                throw new InvalidOperationException("Neke od odabranih tura ne postoje.");

            // Validacija da sve ture pripadaju autoru
            if (tours.Any(t => t.AuthorId != authorId))
                throw new UnauthorizedAccessException("Možeš dodati samo svoje ture u bundle.");

            bundle.Update(dto.Name, dto.Price, dto.TourIds);

            var updated = _bundleRepository.Update(bundle);

            return MapToDto(updated, tours);
        }

        public void Publish(long authorId, long bundleId)
        {
            var bundle = _bundleRepository.GetById(bundleId);
            if (bundle == null)
                throw new KeyNotFoundException("Bundle nije pronađen.");

            if (bundle.AuthorId != authorId)
                throw new UnauthorizedAccessException("Nemaš dozvolu da objavljuješ ovaj bundle.");

            // Provera da postoje minimum 2 published ture
            var tours = _tourRepository.GetByIds(bundle.TourIds);
            var publishedCount = tours.Count(t => t.Status == TourStatus.Published);

            if (publishedCount < 2)
                throw new InvalidOperationException("Bundle mora imati minimum 2 published ture da bi bio objavljen.");

            bundle.Publish();
            _bundleRepository.Update(bundle);
        }

        public void Archive(long authorId, long bundleId)
        {
            var bundle = _bundleRepository.GetById(bundleId);
            if (bundle == null)
                throw new KeyNotFoundException("Bundle nije pronađen.");

            if (bundle.AuthorId != authorId)
                throw new UnauthorizedAccessException("Nemaš dozvolu da arhiviraš ovaj bundle.");

            bundle.Archive();
            _bundleRepository.Update(bundle);
        }

        public void Delete(long authorId, long bundleId)
        {
            var bundle = _bundleRepository.GetById(bundleId);
            if (bundle == null)
                throw new KeyNotFoundException("Bundle nije pronađen.");

            if (bundle.AuthorId != authorId)
                throw new UnauthorizedAccessException("Nemaš dozvolu da brišeš ovaj bundle.");

            if (bundle.Status == BundleStatus.Published)
                throw new InvalidOperationException("Objavljeni bundle ne može biti obrisan, samo arhiviran.");

            _bundleRepository.Delete(bundleId);
        }

        private BundleDto MapToDto(Bundle bundle, List<Tour> tours)
        {
            var totalPrice = tours.Sum(t => t.Price);

            return new BundleDto
            {
                Id = bundle.Id,
                Name = bundle.Name,
                Price = bundle.Price,
                Status = bundle.Status.ToString(),
                AuthorId = bundle.AuthorId,
                TourIds = bundle.TourIds,
                TotalToursPrice = totalPrice,
                CreatedAt = bundle.CreatedAt,
                UpdatedAt = bundle.UpdatedAt
            };
        }
    }
}
