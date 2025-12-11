using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Internal
{
    public interface IUsernameProvider
    {
        Dictionary<long, string> GetNamesByIds(IEnumerable<long> ids);
        string GetNameById(long id);
        
    }
}

