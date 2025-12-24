using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public List<TouristBasicDto> GetTourists(string? query)
        {
            var tourists = _userRepository.GetTourists(query);

            return tourists.Select(u => new TouristBasicDto
            {
                Id = u.Id,
                Username = u.Username
            }).ToList();
        }
    }
}