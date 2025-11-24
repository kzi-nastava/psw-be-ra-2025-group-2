using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;
public interface ITouristObjectRepository
{
    PagedResult<TouristObject> GetPaged(int page, int pageSize);
    TouristObject Create(TouristObject map);
    TouristObject Update(TouristObject map);
    void Delete(long id);
}
