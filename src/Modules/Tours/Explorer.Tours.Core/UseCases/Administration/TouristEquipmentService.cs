using AutoMapper;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.UseCases;
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
        private readonly ITouristEquipmentRepository _repository;
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly IMapper _mapper;

        public TouristEquipmentService(ITouristEquipmentRepository repository, IEquipmentRepository equipmentRepository, IMapper mapper)
        {
            _repository = repository;
            _equipmentRepository = equipmentRepository;
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
            var resultDto = _mapper.Map<TouristEquipmentDto>(result);
            resultDto.Equipments = new List<EquipmentDto>();
            foreach (int id in resultDto.Equipment)
            {
                Equipment eq = _equipmentRepository.Get(id);
                resultDto.Equipments.Add(_mapper.Map<EquipmentDto>(eq));
            }
            return resultDto;
        }

         public PagedResult<EquipmentDto> GetAllEquipment(int page, int pageSize)
        {
            var result = _equipmentRepository.GetPaged(page, pageSize);

            var items = result.Results.Select(_mapper.Map<EquipmentDto>).ToList();
            return new PagedResult<EquipmentDto>(items, result.TotalCount);
        } 

        /*public PagedResult<EquipmentDto> GetAllEquipment(int page, int pageSize)
        {
            var result = _equipmentRepository.GetPaged(page, pageSize);
            return _mapper.Map<PagedResult<EquipmentDto>>(result);
        }*/

        public TouristEquipmentDto Update(TouristEquipmentDto entity)
        {
            var result = _repository.Update(_mapper.Map<TouristEquipment>(entity));
            return _mapper.Map<TouristEquipmentDto>(result);
        }
    }
}
