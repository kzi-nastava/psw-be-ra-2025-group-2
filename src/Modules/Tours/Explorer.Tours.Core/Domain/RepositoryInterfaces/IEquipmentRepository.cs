
using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IEquipmentRepository
{
    PagedResult<Equipment> GetPaged(int page, int pageSize);
    public Equipment Get(long id);
    Equipment Create(Equipment map);
    Equipment Update(Equipment map);
    void Delete(long id);
}