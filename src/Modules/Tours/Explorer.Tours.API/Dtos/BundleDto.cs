using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public record BundleDto
    {
        public long Id { get; init; }
        public string Name { get; init; }
        public decimal Price { get; init; }
        public string Status { get; init; }
        public long AuthorId { get; init; }
        public List<long> TourIds { get; init; }
        public decimal TotalToursPrice { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }

    public record CreateBundleDto
    {
        public string Name { get; init; }
        public decimal Price { get; init; }
        public List<long> TourIds { get; init; }
    }

    public record UpdateBundleDto
    {
        public string Name { get; init; }
        public decimal Price { get; init; }
        public List<long> TourIds { get; init; }
    }
}
