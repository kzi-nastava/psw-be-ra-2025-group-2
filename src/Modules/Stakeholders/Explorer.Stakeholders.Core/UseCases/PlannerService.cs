using AutoMapper;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.API.Internal;
using Explorer.Stakeholders.API.Dtos.Planner;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.Exceptions;
using Explorer.Stakeholders.Core.Domain.Planner;
using Explorer.Stakeholders.Core.Domain.Planner.Services;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Internal;
using Explorer.Tours.API.Public.Administration;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Explorer.Stakeholders.Core.Domain.Planner.PlanEvaluationContext;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class PlannerService : IPlannerService
    {
        private readonly IInternalTourService _internalTourService;
        private readonly IPlannerRepository _plannerRepository;
        private readonly IInternalTokenService _tokenService;
        private readonly IPlanEvaluator _planEvaluator;
        private readonly IMapper _mapper;

        public PlannerService(IInternalTourService internalTourService, IPlannerRepository plannerRepository, IInternalTokenService tokenService, IPlanEvaluator planEvaluator, IMapper mapper)
        {
            _internalTourService = internalTourService;
            _plannerRepository = plannerRepository;
            _tokenService = tokenService;
            _planEvaluator = planEvaluator;
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

            var dayEntry = _plannerRepository.GetByScheduleEntryId(newSchedule.Id);

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

        public IEnumerable<SuggestionDto> EvaluatePlan(long touristId, int month, int? day, int year)
        {
            List<ScheduleEntry> scheduleEntries;

            if(day != null)
            {
                scheduleEntries = _plannerRepository.GetDailyScheduleEntries(touristId, day.Value, month, year);
            }
            else
            {
                scheduleEntries = _plannerRepository.GetMonthlyScheduleEntries(touristId, month, year);
            }

            var tourIds = scheduleEntries.Select(e => e.TourId).Distinct().ToList();

            var tourMetadata = GetMetadataDictionary(_internalTourService.GetMetadataByIds(tourIds));

            var evaluationEntries = GetEvaluationEntries(scheduleEntries, tourMetadata);

            var context = GetContext(evaluationEntries, day, month, year);

            var evaluationResults = _planEvaluator.Evaluate(context);

            var suggestions = evaluationResults.Select(r => new SuggestionDto
            {
                Date = r.Date,
                Kind = r.Kind.ToString(),
                Message = r.Message
            });

            return suggestions.ToList();
        }

        private PlanEvaluationContext GetContext(List<PlanEvaluationEntry> entries, int? day, int month, int year)
        {
            var builder = new PlanEvaluationContextBuilder();

            builder.ForDate(new DateOnly(year, month, day ?? 1))
                .WithTimeScope(day == null ? EvaluationTimeScope.Month : EvaluationTimeScope.Day);

            foreach(var entry in entries)
            {
                builder.AddEntry(entry);
            }

            return builder.Build();
        }

        private Dictionary<long, PlannerSuggestionMetadataDto> GetMetadataDictionary(IEnumerable<PlannerSuggestionMetadataDto> suggestions)
        {
            var ret = new Dictionary<long, PlannerSuggestionMetadataDto>();

            foreach (var suggestion in suggestions)
            {
                ret.Add(suggestion.TourId, suggestion);
            }

            return ret;
        }

        private List<PlanEvaluationEntry> GetEvaluationEntries(List<ScheduleEntry> scheduleEntries, Dictionary<long, PlannerSuggestionMetadataDto> metadata)
        {
            var ret = new List<PlanEvaluationEntry>();

            foreach(var entry in scheduleEntries)
            {
                ret.Add(new PlanEvaluationEntry(
                    metadata[entry.TourId].TourName,
                    entry.ScheduledTime,
                    Minutes.Of(metadata[entry.TourId].TotalDurationMinutes),
                    new GeoLocation(metadata[entry.TourId].FirstKeyPointLatitude, metadata[entry.TourId].FirstKeyPointLongitude),
                    new GeoLocation(metadata[entry.TourId].LastKeyPointLatitude, metadata[entry.TourId].LastKeyPointLongitude)
                    ));
            }

            return ret;
        }

        private void EnrichWithTourNames(DayEntryDto dto)
        {
            var tourIds = dto.Entries.Select(e => e.TourId).Distinct();

            var tourInfos = _internalTourService.GetPartialTourInfos(tourIds).ToDictionary(t => t.Id, t => t.Name);

            foreach (var entry in dto.Entries)
            {
                if (tourInfos.TryGetValue(entry.TourId, out var name))
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

            foreach (var dayEntry in entries)
            {
                foreach (var scheduleEntry in dayEntry.Entries)
                {
                    if (tourInfos.TryGetValue(scheduleEntry.TourId, out var name))
                    {
                        scheduleEntry.TourName = name;
                    }
                }
            }
        }
    }
}
