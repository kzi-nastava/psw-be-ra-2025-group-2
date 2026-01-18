using AutoMapper;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.API.Internal;
using Explorer.Stakeholders.API.Dtos.Planner;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.Exceptions;
using Explorer.Stakeholders.Core.Domain.Planner;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Internal;
using Explorer.Tours.API.Public.Administration;
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
        private readonly IInternalTokenService _tokenService;
        private readonly IMapper _mapper;

        public PlannerService(IInternalTourService internalTourService, IPlannerRepository plannerRepository, IInternalTokenService tokenService, IMapper mapper)
        {
            _internalTourService = internalTourService;
            _plannerRepository = plannerRepository;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public DayEntryDto CreateScheduleEntry(long touristId, CreateScheduleDto newSchedule)
        {
            var tokens = _tokenService.GetPurchasedTourIds(touristId);

            if (!tokens.Contains(newSchedule.TourId))
                throw new TourNotOwnedException($"Tourist does not own the tour with id {newSchedule.TourId}.");

            bool exists = _internalTourService.Exists(newSchedule.TourId);

            if (!exists)
                throw new NotFoundException("Tour not found.");

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
            var dayEntry = _plannerRepository.GetByScheduleEntryId(id);

            if (dayEntry == null)
                throw new NotFoundException("Day entry not found.");

            dayEntry.RemoveScheduleEntry(id);

            if (!dayEntry.Entries.Any())
            {
                _plannerRepository.Delete(dayEntry.Id);
            }

            else
            {
                _plannerRepository.Update(dayEntry);
            }
        }

        public DayEntryDto UpdateScheduleEntry(long touristId, UpdateScheduleDto newSchedule)
        {
            var tokens = _tokenService.GetPurchasedTourIds(touristId);

            if (!tokens.Contains(newSchedule.TourId))
                throw new TourNotOwnedException($"Tourist does not own the tour with id {newSchedule.TourId}.");

            bool exists = _internalTourService.Exists(newSchedule.TourId);

            if (!exists)
                throw new NotFoundException("Tour not found.");

            var dayEntry = _plannerRepository.GetByScheduleEntryId(touristId);

            if (dayEntry == null)
                throw new NotFoundException("Day entry not found.");

            dayEntry.UpdateScheduleEntry(newSchedule.Id, newSchedule.Notes, DateTimeInterval.Of(newSchedule.Start, newSchedule.End), newSchedule.TourId);

            _plannerRepository.Update(dayEntry);

            var ret = _mapper.Map<DayEntryDto>(dayEntry);

            EnrichWithTourNames(ret);

            return ret;
        }

        public DayEntryDto UpdateDayNotes(long touristId, UpdateDayNotesDto newNotes)
        {
            var dayEntry = _plannerRepository.GetById(newNotes.Id);

            if (dayEntry == null)
                throw new NotFoundException("Day entry not found.");

            dayEntry.SetNotes(newNotes.Notes);

            _plannerRepository.Update(dayEntry);

            var ret = _mapper.Map<DayEntryDto>(dayEntry);

            EnrichWithTourNames(ret);

            return ret;
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
