using Explorer.Tours.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Unit.Domain
{
    public class TourAggregateTests
    {
        [Fact]
        public void AddKeyPoint_succeeds()
        {
            var tour = new Tour("Test Tour", "Desc", 3, 1);
            var kp = new KeyPoint(1, "KP1", "Desc", "Secret", "img.png", 45, 19);

            tour.AddKeyPoint(kp);

            tour.KeyPoints.Count.ShouldBe(1);
            tour.KeyPoints[0].Name.ShouldBe("KP1");
        }

        [Fact]
        public void AddKeyPoint_fails_when_duplicateOrdinal()
        {
            var tour = new Tour("Test Tour", "Desc", 3, 1);
            var kp1 = new KeyPoint(1, "KP1", "Desc", "Secret", "img.png", 45, 19);
            var kp2 = new KeyPoint(1, "KP2", "Desc2", "Secret2", "img2.png", 46, 20);

            tour.AddKeyPoint(kp1);

            Should.Throw<InvalidOperationException>(() => tour.AddKeyPoint(kp2));
        }
    }
}