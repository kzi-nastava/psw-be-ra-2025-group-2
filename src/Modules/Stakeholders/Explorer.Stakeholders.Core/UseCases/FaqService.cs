using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos.Help;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.Help;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class FaqService : IFaqService
    {
        private readonly IFaqRepository _repository;

        public FaqService(IFaqRepository repository)
        {
            _repository = repository;
        }

        public List<FaqItemDto> GetActive() => _repository.GetActive().Select(x => new FaqItemDto { Id = x.Id, Category = x.Category, Question = x.Question, Answer = x.Answer, IsActive = x.IsActive }).ToList();

        public FaqItemDto Create(CreateFaqItemDto dto)
        {
            var item = new FaqItem(dto.Category, dto.Question, dto.Answer);
            _repository.Create(item);

            return new FaqItemDto
            {
                Id = item.Id,
                Category = item.Category,
                Question = item.Question,
                Answer = item.Answer,
                IsActive = item.IsActive,
            };
        }

        public FaqItemDto Update(long id, UpdateFaqItemDto dto)
        {
            var item = _repository.Get(id);
            item.Update(dto.Question, dto.Answer);
            _repository.Update(item);

            return new FaqItemDto
            {
                Id = item.Id,
                Category = item.Category,
                Question = item.Question,
                Answer = item.Answer,
                IsActive = item.IsActive,
            };
        }

        public void Deactivate(long id)
        {
            var item = _repository.Get(id);
            item.Deactivate();
            _repository.Update(item);
        }
    }
}
