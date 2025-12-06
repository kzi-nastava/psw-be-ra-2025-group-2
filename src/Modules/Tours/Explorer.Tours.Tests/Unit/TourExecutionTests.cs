using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.Exceptions;
using Explorer.Tours.Core.Domain.Execution;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Unit
{
    // Starts, Abandons, Completes, GetsLastVisitedKPO, QueriesKPV, HasTouristVisited, RecordsActivity

    public class TestKeyPointInfo : IKeyPointInfo
    {
        public int OrdinalNo { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class TourExecutionTests
    {
        [Fact]
        public void Starts()
        {
            // Arrange
            TourExecution execution = new TourExecution(-1, -1, 5);
            execution.State.ShouldBe(TourExecutionState.NotStarted);

            execution.Start();

            execution.State.ShouldBe(TourExecutionState.InProgress);
            execution.LastActivityTimestamp.Date.ShouldBe(DateTime.Today);

            Should.Throw<TourExecutionStateException>(execution.Start);
        }

        [Fact]
        public void Abandons()
        {
            TourExecution execution = new TourExecution(-1, -1, 5);

            Should.Throw<TourExecutionStateException>(execution.Abandon);

            execution.Start();

            execution.Abandon();

            execution.State.ShouldBe(TourExecutionState.Abandoned);

            Should.Throw<TourExecutionStateException>(execution.Abandon);
        }

        [Fact]
        public void Queries_key_points_correctly()
        {
            var keyPointInfo = new List<IKeyPointInfo>()
            {
                new TestKeyPointInfo { OrdinalNo = 1, Latitude = 45, Longitude = 19 },
                new TestKeyPointInfo { OrdinalNo = 2, Latitude = 50, Longitude = 20 },
                new TestKeyPointInfo { OrdinalNo = 3, Latitude = 55, Longitude = 40 }
            };

            TourExecution execution = new TourExecution(-1, -1, 3);

            execution.Start();

            execution.KeyPointVisits.Count.ShouldBe(0);
            execution.GetLastVisitedKeyPointOrdinal().ShouldBe(0);

            execution.QueryKeyPointVisit(keyPointInfo, 45, 19).ShouldBe(1);
            execution.QueryKeyPointVisit(keyPointInfo, 50, 20).ShouldBe(2);

            execution.KeyPointVisits.Count.ShouldBe(2);
            execution.GetLastVisitedKeyPointOrdinal().ShouldBe(2);
            execution.HasTouristVisitedKeyPoint(1).ShouldBe(true);
            execution.HasTouristVisitedKeyPoint(2).ShouldBe(true);
            execution.HasTouristVisitedKeyPoint(3).ShouldBe(false);

            execution.QueryKeyPointVisit(keyPointInfo, 50, 20).ShouldBe(null);
            execution.QueryKeyPointVisit(keyPointInfo, 69, 69).ShouldBe(null);
            execution.QueryKeyPointVisit(keyPointInfo, 55, 40).ShouldBe(3);

            execution.KeyPointVisits.Count.ShouldBe(3);

            execution.GetLastVisitedKeyPointOrdinal().ShouldBe(3);
        }

        [Fact]
        public void Key_point_query_fails_inconsistent_data()
        {
            var keyPointInfo = new List<IKeyPointInfo>()
            {
                new TestKeyPointInfo { OrdinalNo = 1, Latitude = 45, Longitude = 19 },
                new TestKeyPointInfo { OrdinalNo = 2, Latitude = 50, Longitude = 20 },
                new TestKeyPointInfo { OrdinalNo = 3, Latitude = 55, Longitude = 40 }
            };

            TourExecution execution = new TourExecution(-1, -1, 2);

            execution.Start();

            Should.Throw<InvalidDataException>(() => { execution.QueryKeyPointVisit(keyPointInfo, 45, 45); });
        }

        [Fact]
        public void Completes()
        {
            var keyPointInfo = new List<IKeyPointInfo>()
            {
                new TestKeyPointInfo { OrdinalNo = 1, Latitude = 45, Longitude = 19 },
                new TestKeyPointInfo { OrdinalNo = 2, Latitude = 50, Longitude = 20 },
                new TestKeyPointInfo { OrdinalNo = 3, Latitude = 55, Longitude = 40 }
            };

            TourExecution execution = new TourExecution(-1, -1, 3);

            execution.Start();

            execution.QueryKeyPointVisit(keyPointInfo, 45, 19);
            execution.QueryKeyPointVisit(keyPointInfo, 50, 20);

            Should.Throw<InvalidOperationException>(execution.Complete);

            execution.QueryKeyPointVisit(keyPointInfo, 55, 40);

            execution.Complete();
            execution.State.ShouldBe(TourExecutionState.Completed);

            Should.Throw<TourExecutionStateException>(execution.Complete);
        }
    }
}
