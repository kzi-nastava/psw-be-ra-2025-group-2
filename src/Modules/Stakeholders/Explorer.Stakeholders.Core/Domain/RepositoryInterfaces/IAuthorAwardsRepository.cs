using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IAuthorAwardsRepository
    {
        public bool ExistsByYear(int year);
        public PagedResult<AuthorAwards> GetPaged(int page, int pageSize);
        public AuthorAwards Create(AuthorAwards awards);
        public AuthorAwards Update(AuthorAwards awards);
        public void Delete(long id);
    }
}
