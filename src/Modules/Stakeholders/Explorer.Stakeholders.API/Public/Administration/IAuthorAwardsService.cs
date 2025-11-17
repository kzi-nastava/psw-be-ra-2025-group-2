using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public.Administration
{
    public interface IAuthorAwardsService
    {
        public PagedResult<AuthorAwardsDto> GetPaged(int page, int pageSize);
        public AuthorAwardsDto Create(AuthorAwardsDto awards);
        public AuthorAwardsDto Update(AuthorAwardsDto awards);
        public void Delete(long id);
    }
}
