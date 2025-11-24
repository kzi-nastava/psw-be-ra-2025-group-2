using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Administration;
public interface ITouristObjectService
{
    PagedResult<TouristObjectDto> GetPaged(int page, int pageSize);
    TouristObjectDto Create(TouristObjectDto tourObject);
    TouristObjectDto Update(TouristObjectDto tourObject);
    void Delete(long id);
}
