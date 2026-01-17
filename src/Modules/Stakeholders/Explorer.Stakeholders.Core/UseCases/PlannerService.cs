using AutoMapper;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Stakeholders.API.Dtos.Planner;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.Planner;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Domain.Services;
using Explorer.Tours.API.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class PlannerService : IPlannerService
    {
        private readonly IInternalTourService _internalTourService;
        private readonly IPlannerRepository _plannerRepository;
        private readonly ITourOwnershipChecker _ownershipChecker;
        private readonly IMapper _mapper;

        public PlannerService(IInternalTourService internalTourService, IPlannerRepository plannerRepository, ITourOwnershipChecker ownershipChecker, IMapper mapper)
        {
            _internalTourService = internalTourService;
            _plannerRepository = plannerRepository;
            _ownershipChecker = ownershipChecker;
            _mapper = mapper;
        }

        public DayEntryDto CreateScheduleEntry(long touristId, CreateScheduleDto newSchedule)
        {
            _ownershipChecker.CheckOwnership(touristId, newSchedule.TourId);

            var existingDayEntry = _plannerRepository.GetByDate(touristId, newSchedule.Date);

            if(existingDayEntry == null)
            {
                var newEntry = new DayEntry(touristId, newSchedule.Date, newSchedule.DayNotes);
                newEntry.AddScheduleEntry(newSchedule.TourId, newSchedule.Notes, DateTimeInterval.Of(newSchedule.Start, newSchedule.End));

                _plannerRepository.Create(newEntry);

                var ret = _mapper.Map<DayEntryDto>(newEntry);

                EnrichWithTourNames(ret);

                return ret;
            }
            else
            {
                existingDayEntry.AddScheduleEntry(newSchedule.TourId, newSchedule.Notes, DateTimeInterval.Of(newSchedule.Start, newSchedule.End));

                _plannerRepository.Update(existingDayEntry);

                var ret = _mapper.Map<DayEntryDto>(existingDayEntry);
                
                EnrichWithTourNames(ret);

                return ret;
            }
        }

        public IEnumerable<DayEntryDto> GetMonthlySchedule(long touristId, int month, int year)
        {
            if (year < 0)
                throw new ArgumentOutOfRangeException(nameof(year));

            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException(nameof(month));

            var dayEntries = _plannerRepository.GetByMonth(touristId, month, year);

            var ret = _mapper.Map<IEnumerable<DayEntryDto>>(dayEntries);

            EnrichMultipleWithTourNames(ret);

            return ret;
        }

        public void RemoveScheduleEntry(long id)
        {
            throw new NotImplementedException();
        }

        public DayEntryDto UpdateScheduleEntry(long touristId, UpdateScheduleDto newSchedule)
        {
            throw new NotImplementedException();
        }

        private void EnrichWithTourNames(DayEntryDto dto)
        {
            var tourIds = dto.Entries.Select(e => e.TourId).Distinct();

            var tourInfos = _internalTourService.GetPartialTourInfos(tourIds).ToDictionary(t => t.Id, t => t.Name);

            foreach(var entry in dto.Entries)
            {
                if(tourInfos.TryGetValue(entry.TourId, out var name))
                {
                    entry.TourName = name;
                }
            }
        }

        private void EnrichMultipleWithTourNames(IEnumerable<DayEntryDto> entries)
        {
            var tourIds = entries.SelectMany(e => e.Entries).Select(s => s.TourId).Distinct().ToList();

            if (!tourIds.Any()) return;

            var tourInfos = _internalTourService.GetPartialTourInfos(tourIds).ToDictionary(t => t.Id, t => t.Name);

            foreach(var dayEntry in entries)
            {
                foreach(var scheduleEntry in dayEntry.Entries)
                {
                    if(tourInfos.TryGetValue(scheduleEntry.TourId, out var name))
                    {
                        scheduleEntry.TourName = name;
                    }
                }
            }
        }
    }
}
