using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain
{
    public class Vote : ValueObject
    {
        public long UserId { get; private set; }
        public VoteValue Value { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Vote() { }

        public Vote(long userId, VoteValue value)
        {
            UserId = userId;
            Value = value;
            CreatedAt = DateTime.UtcNow;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return UserId;
        }
    }
}
