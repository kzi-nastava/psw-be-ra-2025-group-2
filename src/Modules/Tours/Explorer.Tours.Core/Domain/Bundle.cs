using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class Bundle : Entity
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public BundleStatus Status { get; private set; }
        public long AuthorId { get; init; }
        public List<long> TourIds { get; private set; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; private set; }

        // EF Core treba parameterless constructor
        private Bundle() { }

        public Bundle(string name, decimal price, long authorId, List<long> tourIds)
        {
            Name = name;
            Price = price;
            AuthorId = authorId;
            TourIds = tourIds ?? new List<long>();
            Status = BundleStatus.Draft;
            CreatedAt = DateTime.UtcNow;

            Validate();
        }

        public void Update(string name, decimal price, List<long> tourIds)
        {
            if (Status == BundleStatus.Published)
                throw new InvalidOperationException("Ne možeš menjati objavljeni bundle.");

            Name = name;
            Price = price;
            TourIds = tourIds ?? new List<long>();
            UpdatedAt = DateTime.UtcNow;

            Validate();
        }

        public void Publish()
        {
            if (Status == BundleStatus.Published)
                throw new InvalidOperationException("Bundle je već objavljen.");

            Status = BundleStatus.Published;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Archive()
        {
            if (Status != BundleStatus.Published)
                throw new InvalidOperationException("Samo published bundlovi mogu biti arhivirani.");
            


            Status = BundleStatus.Archived;
            UpdatedAt = DateTime.UtcNow;
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Naziv bundle-a je obavezan.");

            if (Price <= 0)
                throw new ArgumentException("Cena ne može biti negativna.");

            if (TourIds == null || !TourIds.Any())
                throw new ArgumentException("Bundle mora sadržati bar jednu turu.");
        }
    }

    public enum BundleStatus
    {
        Draft,
        Published,
        Archived
    }
}
