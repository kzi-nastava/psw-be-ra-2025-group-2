using System.Collections.Generic;

namespace Explorer.Encounters.API.Internal
{
    public interface IInternalEncounterStatisticsService
    {
        Dictionary<long, EncounterStatisticsData> GetEncounterStatistics(IEnumerable<long> encounterIds);
    }

    public class EncounterStatisticsData
    {
        public long EncounterId { get; set; }
        public string EncounterName { get; set; } = string.Empty;
        public int TotalAttempts { get; set; }
        public int SuccessfulAttempts { get; set; }
    }
}