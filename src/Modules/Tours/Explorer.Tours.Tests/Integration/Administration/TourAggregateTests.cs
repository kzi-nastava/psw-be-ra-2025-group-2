using Explorer.Tours.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Unit.Domain
{
    public class TourKeyPointTests
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

        [Fact]
        public void RemoveKeyPoint_succeeds()
        {
            var tour = new Tour("Test Tour", "Desc", 3, 1);
            var kp = new KeyPoint(1, "KP1", "Desc", "Secret", "img.png", 45, 19);
            tour.AddKeyPoint(kp);

            tour.RemoveKeyPoint(1);

            tour.KeyPoints.Count.ShouldBe(0);
        }

        [Fact]
        public void RemoveKeyPoint_doesNothing_whenOrdinalNotFound()
        {
            var tour = new Tour("Test Tour", "Desc", 3, 1);
            var kp = new KeyPoint(1, "KP1", "Desc", "Secret", "img.png", 45, 19);
            tour.AddKeyPoint(kp);

            tour.RemoveKeyPoint(99);

            tour.KeyPoints.Count.ShouldBe(1);
        }

        [Fact]
        public void UpdateKeyPoint_succeeds()
        {
            var tour = new Tour("Test Tour", "Desc", 3, 1);
            var kp = new KeyPoint(1, "KP1", "Desc", "Secret", "img.png", 45, 19);
            tour.AddKeyPoint(kp);

            var update = new KeyPointUpdate(
                 "Updated KP",
                 "Updated Desc",
                 "Updated Secret",
                 "updated-img.png",
                 50,
                 20
            );

            tour.UpdateKeyPoint(1, update);

            tour.KeyPoints[0].Name.ShouldBe("Updated KP");
            tour.KeyPoints[0].Description.ShouldBe("Updated Desc");
            tour.KeyPoints[0].SecretText.ShouldBe("Updated Secret");
            tour.KeyPoints[0].ImageUrl.ShouldBe("updated-img.png");
            tour.KeyPoints[0].Latitude.ShouldBe(50);
            tour.KeyPoints[0].Longitude.ShouldBe(20);
        }

        [Fact]
        public void UpdateKeyPoint_fails_whenOrdinalNotFound()
        {
            var tour = new Tour("Test Tour", "Desc", 3, 1);

            var update = new KeyPointUpdate(
                 "Updated KP",
                 "Updated Desc",
                 "Updated Secret",
                 "updated-img.png",
                 50,
                 20
             );
            Should.Throw<InvalidOperationException>(() => tour.UpdateKeyPoint(1, update));
        }

        [Fact]
        public void ClearKeyPoints_succeeds()
        {
            var tour = new Tour("Test Tour", "Desc", 3, 1);
            tour.AddKeyPoint(new KeyPoint(1, "KP1", "Desc", "Secret", "img.png", 45, 19));
            tour.AddKeyPoint(new KeyPoint(2, "KP2", "Desc2", "Secret2", "img2.png", 46, 20));

            tour.ClearKeyPoints();

            tour.KeyPoints.Count.ShouldBe(0);
        }

        [Fact]
        public void Length_isZero_whenLessThanTwoKeyPoints()
        {
            var tour = new Tour("Test Tour", "Desc", 3, 1);
            tour.AddKeyPoint(new KeyPoint(1, "KP1", "Desc", "Secret", "img.png", 45, 19));

            tour.LengthKm.ShouldBe(0m);
        }

       
            [Fact]
            public void SetLength_updatesLength_whenValid()
            {
                var tour = new Tour("Test Tour", "Desc", 3, 1);

                tour.SetLength(12.5m);

                tour.LengthKm.ShouldBe(12.5m);
            }

            [Fact]
            public void SetLength_throws_whenNegative()
            {
                var tour = new Tour("Test Tour", "Desc", 3, 1);

                Should.Throw<ArgumentOutOfRangeException>(() => tour.SetLength(-1m));
            }

            [Fact]
            public void SetLength_throws_whenExceedsMax()
            {
                var tour = new Tour("Test Tour", "Desc", 3, 1);

                Should.Throw<ArgumentOutOfRangeException>(() => tour.SetLength(2500m));
            }

            [Fact]
            public void SetLength_throws_whenTourIsArchived()
            {
                var tour = new Tour("Test Tour", "Desc", 3, 1);
                tour.SetStatus(TourStatus.Published);
                tour.Archive(DateTime.UtcNow);

                Should.Throw<InvalidOperationException>(() => tour.SetLength(10m));
            }
        
    


    [Fact]
        public void Length_isZero_afterRemovingKeyPoint_whenLessThanTwoRemain()
        {
            var tour = new Tour("Test Tour", "Desc", 3, 1);
            tour.AddKeyPoint(new KeyPoint(1, "KP1", "Desc", "Secret", "img.png", 45, 19));
            tour.AddKeyPoint(new KeyPoint(2, "KP2", "Desc", "Secret", "img2.png", 46, 20));

            tour.RemoveKeyPoint(2);

            tour.LengthKm.ShouldBe(0m);
        }


    }
}