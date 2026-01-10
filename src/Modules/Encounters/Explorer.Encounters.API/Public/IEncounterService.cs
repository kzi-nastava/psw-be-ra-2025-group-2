using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos.Encounter;
using Explorer.Encounters.API.Dtos.EncounterExecution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Public
{
    public interface IEncounterService
    {
        public EncounterDto Get(long id);
        public PagedResult<EncounterDto> GetPaged(int page, int pageSize);
        public IEnumerable<EncounterDto> GetActive();
        public EncounterDto Create(CreateEncounterDto createDto);
        public EncounterDto Update(UpdateEncounterDto updateDto);
        public void Delete(long id);

        public void MakeActive(long id);
        public void Archive(long id);

        public int GetCount();

        public void CompleteEncounter(long userId, long encounterId);
        void ActivateEncounter(long userId, long encounterId,  double latitude, double longitude);
        (bool IsCompleted, int SecondsInsideZone, int RequiredSeconds, DateTime? CompletionTime) PingLocation(
            long userId,
            long encounterId,
            double latitude,
            double longitude,
            int? deltaSeconds = null
        );

        public (bool IsCompleted, int SecondsInsideZone, int RequiredSeconds, DateTime? CompletionTime) GetExecutionStatus(long userId, long encounterId);
        EncounterExecutionStatusDto PingSocialPresence(long userId, long encounterId, double latitude, double longitude);

    }
}
