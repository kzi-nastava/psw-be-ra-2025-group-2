using Explorer.Encounters.API.Internal;
using Explorer.Stakeholders.API.Internal;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.Execution;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.UseCases.Execution;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace Explorer.Tours.Tests.Unit
{
    public class TourExecutionServiceTourStatsTests
    {
        private readonly Mock<IInternalUserService> _userService = new();
        private readonly Mock<ITourExecutionRepository> _executionRepo = new();
        private readonly Mock<ITourRepository> _tourRepo = new();
        private readonly Mock<IInternalEncounterExecutionService> _encounterInternal = new();
        private readonly Mock<ITourStatisticsRepository> _tourStatsRepo = new();

        private TourExecutionService CreateSut()
        {
            return new TourExecutionService(
                _userService.Object,
                _executionRepo.Object,
                _tourRepo.Object,
                _encounterInternal.Object,
                _tourStatsRepo.Object);
        }

        [Fact]
        public void Proceed_increments_starts_when_starting_new_tour()
        {
            var touristId = 1L;
            var tourId = 55L;

            _userService.Setup(x => x.GetActiveTourIdByUserId(touristId)).Returns(0);

            var tour = CreateTourWithOneKeyPoint(tourId);
            _tourRepo.Setup(x => x.GetByIdAsync(tourId)).ReturnsAsync(tour);

            TourExecution createdExecution = null;

            _executionRepo
                .Setup(x => x.Create(It.IsAny<TourExecution>()))
                .Returns((TourExecution e) =>
                {
                    SetPropertyIfExists(e, "Id", 200L);
                    createdExecution = e;
                    return e;
                });

            _executionRepo.Setup(x => x.Get(200L)).Returns(() => createdExecution);
            _tourRepo.Setup(x => x.GetByIdAsync(tourId)).ReturnsAsync(tour);

            var sut = CreateSut();

            var dto = sut.Proceed(touristId, tourId);

            _tourStatsRepo.Verify(x => x.IncrementStarts(tourId, 1), Times.Once);
            _userService.Verify(x => x.SetActiveTourIdByUserId(touristId, tourId), Times.Once);
            dto.ShouldNotBeNull();
        }

        [Fact]
        public void Proceed_does_not_increment_starts_when_continuing_same_active_tour()
        {
            var touristId = 1L;
            var tourId = 55L;

            _userService.Setup(x => x.GetActiveTourIdByUserId(touristId)).Returns(tourId);

            var activeExec = (TourExecution)FormatterServices.GetUninitializedObject(typeof(TourExecution));
            SetPropertyIfExists(activeExec, "Id", 200L);
            SetPropertyIfExists(activeExec, "TourId", tourId);
            SetPropertyIfExists(activeExec, "TouristId", touristId);
            SetPropertyIfExists(activeExec, "LastActivityTimestamp", DateTime.UtcNow);

            _executionRepo.Setup(x => x.GetActiveByTouristId(touristId)).Returns(activeExec);
            _executionRepo.Setup(x => x.Get(200L)).Returns(activeExec);

            var tour = CreateTourWithOneKeyPoint(tourId);
            _tourRepo.Setup(x => x.GetByIdAsync(tourId)).ReturnsAsync(tour);

            var sut = CreateSut();

            var dto = sut.Proceed(touristId, tourId);

            _tourStatsRepo.Verify(x => x.IncrementStarts(It.IsAny<long>(), It.IsAny<int>()), Times.Never);
            _executionRepo.Verify(x => x.Update(It.IsAny<TourExecution>()), Times.Once);
            dto.ShouldNotBeNull();
        }

        [Fact]
        public void Proceed_throws_when_other_tour_is_active()
        {
            var touristId = 1L;
            var tourId = 55L;

            _userService.Setup(x => x.GetActiveTourIdByUserId(touristId)).Returns(999L);

            var tour = CreateTourWithOneKeyPoint(tourId);
            _tourRepo.Setup(x => x.GetByIdAsync(tourId)).ReturnsAsync(tour);

            var sut = CreateSut();

            Should.Throw<InvalidOperationException>(() => sut.Proceed(touristId, tourId));

            _tourStatsRepo.Verify(x => x.IncrementStarts(It.IsAny<long>(), It.IsAny<int>()), Times.Never);
        }

        private static Tour CreateTourWithOneKeyPoint(long tourId)
        {
            var tour = (Tour)FormatterServices.GetUninitializedObject(typeof(Tour));
            SetPropertyIfExists(tour, "Id", tourId);

            var list = new List<KeyPoint>();
            SetFieldIfExists(tour, "_keyPoints", list);
            SetFieldIfExists(tour, "keyPoints", list);
            SetFieldIfExists(tour, "KeyPoints", list);
            SetPropertyIfExists(tour, "KeyPoints", list);

            var keyPoint = (KeyPoint)FormatterServices.GetUninitializedObject(typeof(KeyPoint));
            SetPropertyIfExists(keyPoint, "Id", 1L);
            SetPropertyIfExists(keyPoint, "OrdinalNo", 1);
            SetPropertyIfExists(keyPoint, "Name", "KP");
            SetPropertyIfExists(keyPoint, "Description", "Desc");
            SetPropertyIfExists(keyPoint, "Latitude", 0.0);
            SetPropertyIfExists(keyPoint, "Longitude", 0.0);
            SetPropertyIfExists(keyPoint, "IsEncounterRequired", false);

            list.Add(keyPoint);

            return tour;
        }

        private static void SetFieldIfExists(object obj, string fieldName, object value)
        {
            var f = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (f != null) f.SetValue(obj, value);
        }

        private static void SetPropertyIfExists(object obj, string propName, object value)
        {
            var p = obj.GetType().GetProperty(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (p != null && p.CanWrite)
            {
                p.SetValue(obj, value);
                return;
            }

            var f = obj.GetType().GetField($"<{propName}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            if (f != null)
            {
                f.SetValue(obj, value);
                return;
            }

            f = obj.GetType().GetField(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (f != null)
                f.SetValue(obj, value);
        }
    }
}
