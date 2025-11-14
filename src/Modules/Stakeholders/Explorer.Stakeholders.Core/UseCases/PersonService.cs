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
            return _mapper.Map<PersonProfileDto>(person);
        }

        public PersonProfileDto UpdateProfile(long userId, UpdatePersonProfileDto dto)
        {
            var person = _repo.GetByUserId(userId);

            if (person == null) throw new Exception("Person not found");

            // Map ONLY updatable fields
            person.Biography = dto.Biography;
            person.Motto = dto.Motto;
            person.ProfileImageUrl = dto.ProfileImageUrl;

            _repo.Update(person);
            return _mapper.Map<PersonProfileDto>(person);
        }
    }
}
