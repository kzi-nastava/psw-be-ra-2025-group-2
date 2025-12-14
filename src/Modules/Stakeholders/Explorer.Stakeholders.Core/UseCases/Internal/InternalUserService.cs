using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases.Internal
{
    public class InternalUserService : IInternalUserService
    {
        private readonly IUserRepository _repository;

        public InternalUserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public long? GetActiveTourIdByUserId(long userId)
        {
            var user = _repository.GetByPersonId(userId);

            if (user == null)
                throw new NotFoundException($"User with UserId {userId} not found.");

            return user.ActiveTourId;
        }

        public long SetActiveTourIdByUserId(long userId, long tourId)
        {
            var user = _repository.GetByPersonId(userId);

            if (user == null)
                throw new NotFoundException($"User with UserId {userId} not found.");

            user.SetActiveTourId(tourId);

            _repository.Update(user);

            return tourId;
        }

        public void ResetActiveTourIdByUserId(long userId)
        {
            var user = _repository.GetByPersonId(userId);

            if (user == null)
                throw new NotFoundException($"User with UserId {userId} not found.");

            user.ResetActiveTourId();

            _repository.Update(user);
        }
        public InternalUserDto? GetById(long userId)
        {
            var user = _repository.Get(userId);
            if (user == null) return null;

            return new InternalUserDto
            {
                Id = user.Id,
                Username = user.Username
            };
        }

    }
}
