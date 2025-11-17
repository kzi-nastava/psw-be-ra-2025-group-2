using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class AuthorAwardsDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public string State { get; set; }
        public DateOnly VotingStartDate { get; set; }
        public DateOnly VotingEndDate { get; set; }
    }
}
