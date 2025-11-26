
using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Dtos;


namespace Explorer.Tours.API.Public.Administration
{
    public interface ITouristEquipmentService
    {
        TouristEquipmentDto GetTouristEquipment(int touristId);
        TouristEquipmentDto Create(TouristEquipmentDto entity);
        TouristEquipmentDto Update(TouristEquipmentDto entity);
        PagedResult<EquipmentDto> GetAllEquipment(int page, int pageSize);

    }
}
