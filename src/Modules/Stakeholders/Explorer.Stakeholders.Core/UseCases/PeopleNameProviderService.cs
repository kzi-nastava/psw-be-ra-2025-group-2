using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.Services
{
    public class PeopleNameProvider : IPeopleNameProvider
    {
        private readonly IUserRepository _userRepository;

        public PeopleNameProvider(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Dictionary<long, string> GetNamesByIds(IEnumerable<long> ids)
        {
            var result = new Dictionary<long, string>();
            foreach (var userId in ids)
            {
                var user = _userRepository.GetByPersonId(userId);
                if (user != null)
                {
                    result[userId] = user.Username;
                }
            }
            return result;
        }
    }
}
