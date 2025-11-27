using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.UseCases.Administration
{
    public class TourProblemService : ITourProblemService
    {
        private readonly ITourProblemRepository _tourProblemRepository;
        private readonly IMapper _mapper;

        public TourProblemService(ITourProblemRepository tourProblemRepository, IMapper mapper)
        {
            _tourProblemRepository = tourProblemRepository;
            _mapper = mapper;
        }

        public TourProblemDto Create(CreateTourProblemDto dto)
        {
            var problem = new TourProblem(dto.TourId, dto.Category, dto.Priority, dto.Description);

            // sačuvaj u bazi preko repozitorijuma
            var created = _tourProblemRepository.Create(problem);

            // mapiraj u DTO koji vraća servis
            return _mapper.Map<TourProblemDto>(created);
        }


        public List<TourProblemDto> GetForCreator()
        {
            var problems = _tourProblemRepository.GetForCreator();
            return problems.Select(p => _mapper.Map<TourProblemDto>(p)).ToList();
        }

        public TourProblemDto Update(TourProblemDto dto)
        {
            var existing = _tourProblemRepository.Get(dto.Id);
            if (existing == null)
            {
                throw new KeyNotFoundException("Problem not found.");
            }

            existing.Category = dto.Category;
            existing.Priority = dto.Priority;
            existing.Description = dto.Description;

            var updated = _tourProblemRepository.Update(existing);
            return _mapper.Map<TourProblemDto>(updated);
        }

        public void Delete(long id)
        {
            _tourProblemRepository.Delete(id);
        }
    }
}
