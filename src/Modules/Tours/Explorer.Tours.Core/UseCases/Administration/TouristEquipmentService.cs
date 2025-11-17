using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.UseCases.Administration
{
    public class TouristEquipmentService : ITouristEquipmentService
    {
        public readonly ITouristEquipmentRepository _repository;
        private readonly IMapper _mapper;

        public TouristEquipmentService(ITouristEquipmentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public TouristEquipmentDto Create(TouristEquipmentDto entity)
        {
            var result = _repository.Create(_mapper.Map<TouristEquipment>(entity));
            return _mapper.Map<TouristEquipmentDto>(result);
        }

        public TouristEquipmentDto GetTouristEquipment(int touristId)
        {
            var result = _repository.GetByUserId(touristId);
            return _mapper.Map<TouristEquipmentDto>(result);
        }

        public TouristEquipmentDto Update(TouristEquipmentDto entity)
        {
            var result = _repository.Update(_mapper.Map<TouristEquipment>(entity));
            return _mapper.Map<TouristEquipmentDto>(result);
        }
    }
}
