using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class UsernameProviderService : IUsernameProvider
    {
        private readonly IUserRepository _userRepository;

        public UsernameProviderService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Dictionary<long, string> GetNamesByIds(IEnumerable<long> ids)
        {
            var result = new Dictionary<long, string>();
            foreach (var id in ids)
            {
                var user = _userRepository.GetByPersonId(id);
                if (user != null && !string.IsNullOrWhiteSpace(user.Username))
                {
                    result[id] = user.Username;
                }
            }
            return result;
        }

        public string GetNameById(long id)
        {
            var user = _userRepository.GetByPersonId(id);
            return user?.Username ?? string.Empty;
        }
    }
}

