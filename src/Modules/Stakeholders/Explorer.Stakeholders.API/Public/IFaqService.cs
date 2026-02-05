using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos.Help;

namespace Explorer.Stakeholders.API.Public
{
    public interface IFaqService
    {
        List<FaqItemDto> GetActive();
        FaqItemDto Create(CreateFaqItemDto dto);
        FaqItemDto Update(long id, UpdateFaqItemDto dto);
        void Deactivate(long id);
    }
}
