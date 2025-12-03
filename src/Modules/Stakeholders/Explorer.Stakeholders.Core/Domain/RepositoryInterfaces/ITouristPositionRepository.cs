using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface ITouristPositionRepository
    {
        public TouristPosition? GetByTouristId(long touristId);
        public TouristPosition CreateOrUpdate(TouristPosition position);
    }
}
