using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class Coupon : AggregateRoot
    {
        public string Code {  get; private set; }
        public int DiscountPercentage { get; private set; }
        public DateTime? ValidUntil { get; private set; }
        public long AuthorId { get; private set; }
        public long? TourId { get; private set; } //Ako je null, onda ce vaziti za sve ture autora

        private Coupon() { }
        public Coupon(int discountPercentage, long authorId, long? tourId, DateTime? validUntil)
        {
            if (discountPercentage < 1 || discountPercentage>100) 
            {
                throw new ArgumentException("Invalid discount percentage");
            }

            if (authorId == 0)
            {
                throw new ArgumentException("Invalid AuthorId");
            }

            if (validUntil.HasValue && validUntil <= DateTime.UtcNow)
            {
                throw new ArgumentException("ValidUntil must be in the future");
            }

            Code = GenerateCode();
            DiscountPercentage = discountPercentage;
            AuthorId = authorId;
            TourId = tourId;
            ValidUntil = validUntil;
        }

        public bool IsValid()
        {
            return !ValidUntil.HasValue || ValidUntil.Value >= DateTime.UtcNow;
        }

        private static string GenerateCode()
        {
            return Guid.NewGuid().ToString("N")[..8].ToUpper();
        }

        public void Update(int discountPercentage, long? tourId, DateTime? validUntil)
        {
            if (discountPercentage < 1 || discountPercentage > 100)
            {
                throw new ArgumentException("Invalid discount percentage");
            }
            if (validUntil.HasValue && validUntil <= DateTime.UtcNow.Date)
            {
                throw new ArgumentException("ValidUntil must be in the future");
            }

            DiscountPercentage = discountPercentage;
            TourId = tourId;
            ValidUntil = validUntil;
        }
    }
}
