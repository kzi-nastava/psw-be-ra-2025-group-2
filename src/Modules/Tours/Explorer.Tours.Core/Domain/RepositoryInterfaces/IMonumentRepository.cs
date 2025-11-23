using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IMonumentRepository
{
    PagedResult<Monument> GetPaged(int page, int pageSize);
    Monument Create(Monument map);
    Monument Update(Monument map);
    void Delete(long id);
}
