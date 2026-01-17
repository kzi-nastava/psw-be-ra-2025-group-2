using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Dtos.Internal;
using Explorer.Tours.API.Internal;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.UseCases
{
    public class InternalTourService : IInternalTourService
    {
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public InternalTourService(ITourRepository tourRepository, IMapper mapper)
        {
            _tourRepository = tourRepository;
            _mapper = mapper;
        }

        public FullTourInfoDto GetFullTourInfo(long tourId)
        {
            var tour = _tourRepository.GetByIdAsync(tourId).Result;
            if (tour == null)
                throw new ArgumentException("Invalid tour Id.");

            return new FullTourInfoDto
            {
                Id = tour.Id,
                Name = tour.Name,
                Difficulty = tour.Difficulty,
                LengthKm = tour.LengthKm,
                WalkingMinutes = tour.Durations.Where(d => d.TransportType == TransportType.Walking).FirstOrDefault()?.Minutes ?? 0,
                BicycleMinutes = tour.Durations.Where(d => d.TransportType == TransportType.Bicycle).FirstOrDefault()?.Minutes ?? 0,
                CarMinutes = tour.Durations.Where(d => d.TransportType == TransportType.Car).FirstOrDefault()?.Minutes ?? 0,

                Equipment = _mapper.Map<List<EquipmentDto>>(tour.Equipment),

                FirstKeyPointLatitude = tour.KeyPoints.First().Latitude,
                FirstKeyPointLongitude = tour.KeyPoints.First().Longitude
            };
        }

        public IEnumerable<PartialTourInfoDto> GetPartialTourInfos(IEnumerable<long> tourIds)
        {
            var tours = _tourRepository.GetByIds(tourIds);

            var ret = new List<PartialTourInfoDto>();

            foreach (var tour in tours)
            {
                ret.Add(new PartialTourInfoDto
                {
                    Id = tour.Id,
                    Name = tour.Name
                });
            }

            return ret;
        }
    }
}
