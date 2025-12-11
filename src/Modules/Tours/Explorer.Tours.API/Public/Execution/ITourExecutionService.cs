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
        public bool IsFinishedEnough(long touristId, long tourId);
        public TourExecutionDto Proceed (long touristId, long tourId);
        public void Abandon(long touristId, long executionId);
        public void Complete(long touristId, long executionId);
        public KeyPointVisitResponseDto QueryKeyPointVisit(long touristId, long executionId, PositionDto position);
        public TourExecutionDto GetExecutionData(long touristId, long executionId);
    }
}
