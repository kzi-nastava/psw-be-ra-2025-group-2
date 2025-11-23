using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _repo;
        private readonly IMapper _mapper;

        public PersonService(IPersonRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public PersonProfileDto GetProfile(long userId)
        {
            var person = _repo.GetByUserId(userId);
            if (person == null)
                throw new KeyNotFoundException($"Person with userId {userId} not found");
            return _mapper.Map<PersonProfileDto>(person);
        }

        public PersonProfileDto UpdateProfile(long userId, PersonProfileDto dto)
        {
            var person = _repo.GetByUserId(userId);

            if (person == null) throw new Exception("Person not found");

            _mapper.Map(dto, person);
            person.Validate();

            _repo.Update(person);
            return _mapper.Map<PersonProfileDto>(person);
        }
    }
}
