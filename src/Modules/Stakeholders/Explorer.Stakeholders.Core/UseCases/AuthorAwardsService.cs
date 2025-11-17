using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
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
    public class AuthorAwardsService : IAuthorAwardsService
    {
        private readonly IAuthorAwardsRepository _awardsRepository;
        private readonly IMapper _mapper;

        public AuthorAwardsService(IAuthorAwardsRepository repository, IMapper mapper)
        {
            _awardsRepository = repository;
            _mapper = mapper;
        }

        public PagedResult<AuthorAwardsDto> GetPaged(int page, int pageSize)
        {
            var result = _awardsRepository.GetPaged(page, pageSize);

            var items = result.Results.Select(_mapper.Map<AuthorAwardsDto>).ToList();
            return new PagedResult<AuthorAwardsDto>(items, result.TotalCount);
        }

        public AuthorAwardsDto Update(AuthorAwardsDto awards)
        {
            var result = _awardsRepository.Update(_mapper.Map<AuthorAwards>(awards));
            return _mapper.Map<AuthorAwardsDto>(result);
        }

        public AuthorAwardsDto Create(AuthorAwardsDto awards)
        {
            var result = _awardsRepository.Create(_mapper.Map<AuthorAwards>(awards));
            return _mapper.Map<AuthorAwardsDto>(result);
        }

        public void Delete(long id)
        {
            _awardsRepository.Delete(id);
        }
    }
}
