using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Stakeholders.Core.Domain.Planner.Services;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.BuildingBlocks.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.Planner
{
    public class PlanEvaluator : IPlanEvaluator
    {
        private const double MaxTouristAirLineVelocity = 0.8;

        // Minimalno vreme koje je potrebno da prodje izmedju dva uzastopna konflikta udaljenosti, da se za lanac od 10 slotova u takvom konfliktu ne bi vratilo 9 rezultata
        private readonly TimeSpan MinDistanceConflictInterval = TimeSpan.FromHours(6);

        public PlanEvaluator()
        {

        }

        public IEnumerable<PlanEvaluationResult> Evaluate(PlanEvaluationContext ctx)
        {
            var entries = ctx.Entries;
            var scope = ctx.Scope;
            var date = ctx.Date;

            List<PlanEvaluationResult> result = new List<PlanEvaluationResult>();

            entries = (scope == EvaluationTimeScope.Day) ? entries.Where(e => DateOnly.FromDateTime(e.Slot.Start) == date).ToList() : entries;

            Dictionary<DateOnly, List<PlanEvaluationEntry>> entriesByDate = SegmentByDate(entries);

            // 1. Ambicioznost vremena

            foreach (var entry in entries)
            {
                if (entry.Slot.Duration.TotalMinutes < 0.8 * entry.Minutes)
                {
                    result.Add(PlanEvaluationResult.WithSmallTimeSlotIssue(DateOnly.FromDateTime(entry.Slot.Start), entry.TourName, entry.Minutes, entry.Slot));
                }
            }


            // 2. Ambicioznost udaljenosti

            if (entries.Count > 1)
            {
                DateTime lastConflictFirstTourEnd = default;

                for (int i = 0; i < entries.Count - 1; i++)
                {
                    Distance distance = Haversine.CalculateDistance(entries[i].LastKeyPointCoordinates, entries[i + 1].FirstKeyPointCoordinates);
                    int minutesBetweenStarts = (int)(entries[i + 1].Slot.Start - entries[i].Slot.End).TotalMinutes;
                    
                    double requiredVelocity = (minutesBetweenStarts == 0) ? -1 : distance.ToKilometers() / minutesBetweenStarts;
                    
                    if (requiredVelocity > MaxTouristAirLineVelocity)
                    {
                        if (entries[i].Slot.End - lastConflictFirstTourEnd >= MinDistanceConflictInterval)
                        {
                            result.Add(PlanEvaluationResult.WithDistanceIssue(DateOnly.FromDateTime(entries[i].Slot.Start), entries[i].TourName, entries[i + 1].TourName, distance));
                        }
                        lastConflictFirstTourEnd = entries[i].Slot.End;
                    }
                    else if (requiredVelocity == -1)
                    {
                        if (entries[i].Slot.End - lastConflictFirstTourEnd >= MinDistanceConflictInterval && distance > Distance.FromMeters(500))
                        {
                            result.Add(PlanEvaluationResult.WithDistanceIssue(DateOnly.FromDateTime(entries[i].Slot.Start), entries[i].TourName, entries[i + 1].TourName, distance));
                        }
                        lastConflictFirstTourEnd = entries[i].Slot.End;
                    }
                }
            }

            // 3. Ambicioznost broja tura u danu

            foreach(var key in entriesByDate.Keys)
            {
                if (entriesByDate[key].Count > 3)
                {
                    result.Add(PlanEvaluationResult.WithOverbookingIssue(key, entriesByDate[key].Select(e => e.TourName)));
                }
            }

            return result.OrderBy(r => r.Date).ToList();
        }


        private Dictionary<DateOnly, List<PlanEvaluationEntry>> SegmentByDate(IEnumerable<PlanEvaluationEntry> entries)
        {
            var ret = new Dictionary<DateOnly, List<PlanEvaluationEntry>>();

            foreach(var entry in entries)
            {
                var date = DateOnly.FromDateTime(entry.Slot.Start);
                
                if(!ret.TryGetValue(date, out var _))
                {
                    ret[date] = new List<PlanEvaluationEntry>();
                }

                ret[date].Add(entry);
            }

            return ret;
        }
    }
}
