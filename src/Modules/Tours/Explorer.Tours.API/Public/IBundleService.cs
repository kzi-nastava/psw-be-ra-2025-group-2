using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public
{
    public interface IBundleService
    {
        BundleDto Create(long authorId, CreateBundleDto dto);
        BundleDto GetById(long id);
        List<BundleDto> GetByAuthorId(long authorId);
        BundleDto Update(long authorId, long bundleId, UpdateBundleDto dto);
        void Publish(long authorId, long bundleId);
        void Archive(long authorId, long bundleId);
        void Delete(long authorId, long bundleId);
    }
}
