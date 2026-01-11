using Explorer.Payments.Core.Domain;
using Shouldly;
using System;

namespace Explorer.Payments.Tests.Unit
{
    public class CouponUnitTests
    {
        [Fact]
        public void Creates_coupon_with_valid_data()
        {
            // Arrange & Act
            var coupon = new Coupon(20, 5, 10, DateTime.UtcNow.AddDays(30));

            // Assert
            coupon.DiscountPercentage.ShouldBe(20);
            coupon.AuthorId.ShouldBe(5);
            coupon.TourId.ShouldBe(10);
            coupon.Code.ShouldNotBeNullOrWhiteSpace();
            coupon.Code.Length.ShouldBe(8);
        }

        [Fact]
        public void Creates_author_wide_coupon()
        {
            // Arrange & Act
            var coupon = new Coupon(15, 5, null, DateTime.UtcNow.AddDays(30));

            // Assert
            coupon.DiscountPercentage.ShouldBe(15);
            coupon.AuthorId.ShouldBe(5);
            coupon.TourId.ShouldBeNull();
        }

        [Fact]
        public void Generates_unique_code()
        {
            // Arrange & Act
            var coupon1 = new Coupon(20, 5, 10, DateTime.UtcNow.AddDays(30));
            var coupon2 = new Coupon(20, 5, 10, DateTime.UtcNow.AddDays(30));

            // Assert
            coupon1.Code.ShouldNotBe(coupon2.Code);
        }

        [Fact]
        public void Creation_fails_with_invalid_discount_percentage_too_low()
        {
            // Act & Assert
            Should.Throw<ArgumentException>(() =>
                new Coupon(0, 5, 10, DateTime.UtcNow.AddDays(30)));
        }

        [Fact]
        public void Creation_fails_with_invalid_discount_percentage_too_high()
        {
            // Act & Assert
            Should.Throw<ArgumentException>(() =>
                new Coupon(101, 5, 10, DateTime.UtcNow.AddDays(30)));
        }

        [Fact]
        public void Creation_fails_with_invalid_author_id()
        {
            // Act & Assert
            Should.Throw<ArgumentException>(() =>
                new Coupon(20, 0, 10, DateTime.UtcNow.AddDays(30)));
        }

        [Fact]
        public void Creation_fails_with_past_valid_until_date()
        {
            // Act & Assert
            Should.Throw<ArgumentException>(() =>
                new Coupon(20, 5, 10, DateTime.UtcNow.AddDays(-1)));
        }

        [Fact]
        public void IsValid_returns_true_for_future_date()
        {
            // Arrange
            var coupon = new Coupon(20, 5, 10, DateTime.UtcNow.AddDays(30));

            // Act & Assert
            coupon.IsValid().ShouldBeTrue();
        }

        [Fact]
        public void IsValid_returns_false_for_expired_coupon()
        {
            // Arrange
            var coupon = new Coupon(20, 5, 10, DateTime.UtcNow.AddSeconds(1));
            System.Threading.Thread.Sleep(1500); // Wait for expiration

            // Act & Assert
            coupon.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Updates_coupon_with_valid_data()
        {
            // Arrange
            var coupon = new Coupon(20, 5, 10, DateTime.UtcNow.AddDays(30));
            var newValidUntil = DateTime.UtcNow.AddDays(60);

            // Act
            coupon.Update(25, 15, newValidUntil);

            // Assert
            coupon.DiscountPercentage.ShouldBe(25);
            coupon.TourId.ShouldBe(15);
            coupon.ValidUntil.ShouldBe(newValidUntil);
        }

        [Fact]
        public void Update_fails_with_invalid_discount()
        {
            // Arrange
            var coupon = new Coupon(20, 5, 10, DateTime.UtcNow.AddDays(30));

            // Act & Assert
            Should.Throw<ArgumentException>(() =>
                coupon.Update(0, 10, DateTime.UtcNow.AddDays(30)));
        }

        [Fact]
        public void Update_allows_changing_to_author_wide_coupon()
        {
            // Arrange
            var coupon = new Coupon(20, 5, 10, DateTime.UtcNow.AddDays(30));

            // Act
            coupon.Update(25, null, DateTime.UtcNow.AddDays(60));

            // Assert
            coupon.TourId.ShouldBeNull();
            coupon.DiscountPercentage.ShouldBe(25);
        }
    }
}