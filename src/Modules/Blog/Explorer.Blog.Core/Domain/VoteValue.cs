using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain
{
    public class VoteValue : ValueObject
    {
        public int Value { get; private set; }

        public static VoteValue Upvote => new VoteValue(1);
        public static VoteValue Downvote => new VoteValue(-1);

        private VoteValue() { }

        private VoteValue(int value)
        {
            if(value != 1 && value != -1)
            {
                throw new ArgumentException("Vote value must be +1 or -1");
            }
            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
