using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.Core.Domain
{
    public enum BlogState
    {
        Draft = 0,
        Published = 1,
        Active = 2,
        Famous = 3,
        Closed = 4,
        Archived = 5
    }
}
