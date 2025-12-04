using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public.Execution
{
    public interface ITourExecutionService
    {
        public long Proceed (long touristId, long tourId);
        public void Abandon(long touristId, long executionId);
        public void Complete(long touristId, long executionId);
        public void ArriveAtKeyPointOrdinal(long touristId, long executionId, int keyPointOrdinal);

        public TourExecutionDto GetExecutionData(long touristId, long executionId);
    }
}
