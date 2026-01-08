using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class TourSale : AggregateRoot
    {
        public long AuthorId { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public int DiscountPercentage { get; private set; }

        private readonly List<long> _tourIds = new();
        public IReadOnlyList<long> TourIds => _tourIds.AsReadOnly();

        private TourSale() { }

        public TourSale(long authorId, DateTime startDate, DateTime endDate, int discountPercentage, IEnumerable<long> tourIds)
        {
            AuthorId = authorId;
            SetPeriod(startDate, endDate);
            SetDiscount(discountPercentage);
            SetTours(tourIds);
        }

        public void Update(DateTime startDate, DateTime endDate, int discountPercentage, IEnumerable<long> tourIds)
        {
            SetPeriod(startDate, endDate);
            SetDiscount(discountPercentage);
            SetTours(tourIds);
        }

        private void SetPeriod(DateTime start, DateTime end)
        {
            if (end <= start)
                throw new ArgumentException("End date must be after start date.");

            if ((end - start).TotalDays > 14)
                throw new ArgumentException("Sale duration cannot exceed 14 days.");

            StartDate = start;
            EndDate = end;
        }

        private void SetDiscount(int percentage)
        {
            if (percentage <= 0 || percentage > 90)
                throw new ArgumentException("Discount must be between 1 and 90 percent.");

            DiscountPercentage = percentage;
        }

        private void SetTours(IEnumerable<long> tourIds)
        {
            if (!tourIds.Any())
                throw new ArgumentException("At least one tour must be selected.");

            _tourIds.Clear();
            _tourIds.AddRange(tourIds.Distinct());
        }

        public bool IsActive(DateTime now)
            => now >= StartDate && now <= EndDate;
    }
}
