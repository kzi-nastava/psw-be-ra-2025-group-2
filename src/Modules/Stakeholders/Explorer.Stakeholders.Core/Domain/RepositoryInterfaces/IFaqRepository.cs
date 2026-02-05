using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain.Help;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IFaqRepository
    {
        List<FaqItem> GetActive();
        FaqItem Get(long id);
        FaqItem Create(FaqItem item);
        FaqItem Update(FaqItem item);
    }
}
