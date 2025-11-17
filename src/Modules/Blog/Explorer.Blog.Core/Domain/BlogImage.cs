using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain
{
    public class BlogImage : ValueObject
    {
        public string Url { get; }

        public BlogImage(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Image URL cannot be empty.");

            Url = url;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Url;
        }
    }
}
