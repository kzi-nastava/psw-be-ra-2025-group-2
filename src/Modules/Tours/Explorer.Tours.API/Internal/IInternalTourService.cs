using Explorer.Tours.API.Dtos.Internal;
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
    }
}
