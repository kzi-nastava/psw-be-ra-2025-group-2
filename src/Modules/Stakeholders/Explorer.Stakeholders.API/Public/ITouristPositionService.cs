using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public
{
    public interface ITouristPositionService
    {
        TouristPositionDto GetByTouristId(long touristId);
        TouristPositionDto Update(long touristId, TouristPositionDto position);
    }
}
