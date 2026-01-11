using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos.Encounter;
using Explorer.Encounters.API.Dtos.TouristProgress;
using Explorer.Encounters.API.Dtos.EncounterExecution;
using System;
using System.Collections.Generic;

namespace Explorer.Encounters.API.Public
{
    public interface IEncounterService
    {
        EncounterDto Get(long id);
        PagedResult<EncounterDto> GetPaged(int page, int pageSize);
        IEnumerable<EncounterDto> GetActive();
        EncounterDto Create(CreateEncounterDto createDto);
        EncounterDto Update(UpdateEncounterDto updateDto);
        void Delete(long id);
        void MakeActive(long id);
        void Archive(long id);
        int GetCount();
        void CompleteEncounter(long userId, long encounterId);
        EncounterDto CreateByTourist(long userId, CreateEncounterDto createDto);
        TouristProgressDto GetMyProgress(long userId);
        void ActivateEncounter(long userId, long encounterId, double latitude, double longitude);
        (bool IsCompleted, int SecondsInsideZone, int RequiredSeconds, DateTime? CompletionTime) PingLocation(
            long userId,
            long encounterId,
            double latitude,
            double longitude,
            int? deltaSeconds = null
        );
        (bool IsCompleted, int SecondsInsideZone, int RequiredSeconds, DateTime? CompletionTime) GetExecutionStatus(
            long userId,
            long encounterId
        );
        EncounterExecutionStatusDto PingSocialPresence(long userId, long encounterId, double latitude, double longitude);
    }
}
