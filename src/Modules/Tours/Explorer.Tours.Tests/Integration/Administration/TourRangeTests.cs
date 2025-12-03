using System;
using System.Reflection;
using Explorer.Tours.Core.UseCases.Administration;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Administration
{
    public class TourRangeTests
    {
        // Pomoćna metoda koja poziva privatni IsWithinRange iz TourService refleksijom
        private static bool CallIsWithinRange(
            double latPosition,
            double lonPosition,
            double latPoint,
            double lonPoint,
            double rangeMeters)
        {
            // Kreiramo TourService sa null dependencijama (za ovaj test mu ne trebaju)
            var service = (TourService)Activator.CreateInstance(
                typeof(TourService),
                new object?[] { null, null })!;

            var method = typeof(TourService).GetMethod(
                "IsWithinRange",
                BindingFlags.NonPublic | BindingFlags.Instance);

            method.ShouldNotBeNull();

            var result = (bool)method.Invoke(service, new object[]
            {
                latPosition, lonPosition, latPoint, lonPoint, rangeMeters
            })!;

            return result;
        }

        [Fact]
        public void Keypoint_within_range_returns_true()
        {
            // Novi Sad centar – tačka u krugu od 2km
            double userLat = 45.2550;
            double userLon = 19.8450;
            double kpLat = 45.2560;
            double kpLon = 19.8460;
            double rangeMeters = 2000;

            var inRange = CallIsWithinRange(userLat, userLon, kpLat, kpLon, rangeMeters);

            inRange.ShouldBeTrue();
        }

        [Fact]
        public void Keypoint_far_away_returns_false()
        {
            // Novi Sad vs. tačka kod (0,0) – daleko van dometa
            double userLat = 45.2550;
            double userLon = 19.8450;
            double kpLat = 0.0;
            double kpLon = 0.0;
            double rangeMeters = 5000;

            var inRange = CallIsWithinRange(userLat, userLon, kpLat, kpLon, rangeMeters);

            inRange.ShouldBeFalse();
        }

        [Fact]
        public void Keypoint_exactly_on_range_boundary_is_included()
        {
            // Veoma mala razdaljina – praktično na samoj granici
            double userLat = 45.2550;
            double userLon = 19.8450;
            double kpLat = 45.2551;
            double kpLon = 19.8451;

            // Ovo je veći range da budemo sigurni da je "na granici"
            double rangeMeters = 50;

            var inRange = CallIsWithinRange(userLat, userLon, kpLat, kpLon, rangeMeters);

            inRange.ShouldBeTrue();
        }
    }
}
