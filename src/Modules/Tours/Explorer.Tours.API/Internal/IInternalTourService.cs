using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Internal
{
    public interface IInternalTourService
    {
        IEnumerable<PartialTourInfoDto> GetPartialTourInfos(IEnumerable<long> tourIds);

        IEnumerable<PlannerSuggestionMetadataDto> GetMetadataByIds(IEnumerable<long> tourIds);
        bool Exists(long tourId);
    }
}
