using AutoMapper.Configuration.Annotations;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain
{
    public enum EncounterState
    {
        Draft = 0,
        Active = 1,
        Archived = 2
    };

    public enum EncounterType
    {
        Social = 0,
        Location = 1,
        Miscellaneous = 2
    }

    public class Encounter : AggregateRoot
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public GeoLocation Location { get; private set; }
        public ExperiencePoints XP { get; private set; }

        public EncounterState State { get; private set; }
        public EncounterType Type { get; private set; }

        private Encounter() { }

        public Encounter(string name, string description, GeoLocation location, ExperiencePoints xp, EncounterType type)
        {
            Name = name;
            Description = description;
            Location = location;
            XP = xp;
            Type = type;

            State = EncounterState.Draft;

            Validate();
        }

        public void Update(string name, string description, GeoLocation location, ExperiencePoints xp, EncounterType type)
        {
            if (State != EncounterState.Draft)
                throw new InvalidOperationException("Cannot update an encounter which is not in a draft state.");

            Name = name;
            Description = description;
            Location = location;
            XP = xp;
            Type = type;

            Validate();
        }

        public void MakeActive()
        {
            if (State == EncounterState.Active)
                throw new InvalidOperationException("Cannot activate an encounter which is already active.");

            State = EncounterState.Active;
        }

        public void Archive()
        {
            if (State != EncounterState.Active)
                throw new InvalidOperationException("Cannot archive an encounter which is not active.");

            State = EncounterState.Archived;
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new EntityValidationException("Name cannot be empty.");

            if (string.IsNullOrWhiteSpace(Description))
                throw new EntityValidationException("Description cannot be empty.");

            if (Location == null)
                throw new EntityValidationException("Location cannot be null.");

            if (XP == null)
                throw new EntityValidationException("XP cannot be null.");
        }

    }
}
