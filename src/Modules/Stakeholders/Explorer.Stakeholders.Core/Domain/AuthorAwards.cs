using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Explorer.Stakeholders.Core.Domain
{
    public class AuthorAwards : Entity
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public int Year { get; private set; }
        public AwardsState State { get; private set; }
        public DateOnly VotingStartDate { get; private set; }
        public DateOnly VotingEndDate { get; private set; }

        public AuthorAwards(string name, string? description, int year, AwardsState state, DateOnly votingStartDate, DateOnly votingEndDate)
        {
            Name = name;
            Description = description;
            Year = year;
            State = state;
            VotingStartDate = votingStartDate;
            VotingEndDate = votingEndDate;

            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Invalid awards name.");
            if (Year < DateTime.Today.Year) throw new ArgumentException("Invalid year");
            if (!Enum.IsDefined(typeof(AwardsState), State)) throw new ArgumentException("Invalid awards state.");
            if (VotingStartDate.Year < DateTime.Today.Year || VotingEndDate.Year < DateTime.Today.Year) throw new ArgumentException("Invalid voting start date.");
            if (VotingStartDate > VotingEndDate) throw new ArgumentException("Invalid voting dates.");
        }
    }

    public enum AwardsState
    {
        Draft,
        Final
    }
}
