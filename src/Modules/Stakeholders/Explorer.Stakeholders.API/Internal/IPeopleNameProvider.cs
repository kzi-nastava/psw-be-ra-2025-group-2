using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Internal
{
    public interface IPeopleNameProvider
    {
        Dictionary<long, string> GetNamesByIds(IEnumerable<long> ids);
    }
}
