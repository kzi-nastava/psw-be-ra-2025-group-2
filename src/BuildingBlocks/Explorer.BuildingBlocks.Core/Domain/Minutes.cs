using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.BuildingBlocks.Core.Domain
{
    public class Minutes : ValueObject
    {
        public int Amount { get; }

        public static Minutes Zero => new Minutes(0);

        private Minutes(int amount)
        {

            Amount = amount;
        }

        public static Minutes Of(int amount)
        {
            return new Minutes(amount);
        }

        public static Minutes operator +(Minutes a, Minutes b)
        {
            return new Minutes(a.Amount + b.Amount);
        }

        public static Minutes operator -(Minutes a, Minutes b)
        {
            return new Minutes(a.Amount - b.Amount);
        }

        public static bool operator <(Minutes a, Minutes b) => a.Amount < b.Amount;
        public static bool operator >(Minutes a, Minutes b) => a.Amount > b.Amount;

        public static bool operator <=(Minutes a, Minutes b) => a.Amount <= b.Amount;
        public static bool operator >=(Minutes a, Minutes b) => a.Amount >= b.Amount;

        public static bool operator ==(Minutes a, Minutes b) => a.Equals(b);
        public static bool operator !=(Minutes a, Minutes b) => !(a == b);

        public static implicit operator int(Minutes m) => m.Amount;
        public static explicit operator Minutes(int amount) => Of(amount);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            throw new NotImplementedException();
        }
    }
}
