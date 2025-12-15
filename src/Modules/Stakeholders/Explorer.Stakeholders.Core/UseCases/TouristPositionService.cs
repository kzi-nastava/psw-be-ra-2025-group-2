using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class TouristPositionService : ITouristPositionService
    {
        private readonly ITouristPositionRepository _repository;
        private readonly IMapper _mapper;

        public TouristPositionService(ITouristPositionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public TouristPositionDto GetByTouristId(long touristId)
        {
            var position = _repository.GetByTouristId(touristId);

            if (position == null)
                throw new NotFoundException("Tourist has no reported locations.");

            return _mapper.Map<TouristPositionDto>(position);
        }

        public TouristPositionDto Update(long touristId, TouristPositionDto position)
        {
            position.TouristId = touristId;
            var result = _repository.CreateOrUpdate(_mapper.Map<TouristPosition>(position));

            return position;
        }
    }
}
