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
