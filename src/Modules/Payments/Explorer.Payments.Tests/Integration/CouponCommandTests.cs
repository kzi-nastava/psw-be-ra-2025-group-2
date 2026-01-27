using Explorer.API.Controllers.Author;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Explorer.Payments.Tests.Integration
{
    [Collection("Sequential")]
    public class CouponCommandTests : BaseStakeholdersIntegrationTest
    {
        public CouponCommandTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates_coupon_for_specific_tour()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateControllerWithRole(scope, "-11", "author");
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            var createDto = new CouponCreateDto
            {
                DiscountPercentage = 20,
                TourId = -2,
                ValidUntil = DateTime.UtcNow.AddDays(30).AddMinutes(1)
            };

            // Act
            var actionResult = controller.Create(createDto);

            // PRIVREMENO: Ispiši šta se vraća
            if (actionResult.Result is BadRequestObjectResult badRequest)
            {
                throw new Exception($"BadRequest: {badRequest.Value}");
            }
            var result = actionResult.Result.ShouldBeOfType<OkObjectResult>();
            var coupon = result.Value.ShouldBeOfType<CouponDto>();

            // Assert
            coupon.ShouldNotBeNull();
            coupon.Code.ShouldNotBeNullOrWhiteSpace();
            coupon.DiscountPercentage.ShouldBe(20);
            coupon.TourId.ShouldBe(-2);
            coupon.AuthorId.ShouldBe(-11);

            var entity = dbContext.Coupons.FirstOrDefault(c => c.Code == coupon.Code);
            entity.ShouldNotBeNull();
            entity.DiscountPercentage.ShouldBe(20);
        }

        [Fact]
        public void Creates_author_wide_coupon()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateControllerWithRole(scope, "-11", "author");

            var createDto = new CouponCreateDto
            {
                DiscountPercentage = 15,
                TourId = null,
                ValidUntil = DateTime.UtcNow.AddDays(60).AddMinutes(1)
            };

            // Act
            var actionResult = controller.Create(createDto);
            var result = actionResult.Result.ShouldBeOfType<OkObjectResult>();
            var coupon = result.Value.ShouldBeOfType<CouponDto>();

            // Assert
            coupon.TourId.ShouldBeNull();
            coupon.AuthorId.ShouldBe(-11);
            coupon.DiscountPercentage.ShouldBe(15);
        }

        [Fact]
        public void Create_fails_when_tour_does_not_exist()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateControllerWithRole(scope, "-11", "author");

            var createDto = new CouponCreateDto
            {
                DiscountPercentage = 20,
                TourId = 99999,
                ValidUntil = DateTime.UtcNow.AddDays(30)
            };

            // Act & Assert
            var actionResult = controller.Create(createDto);
            actionResult.Result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Create_fails_when_tour_belongs_to_different_author()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateControllerWithRole(scope, "-11", "author");

            var toursContext = scope.ServiceProvider.GetRequiredService<Explorer.Tours.Infrastructure.Database.ToursContext>();

            var tourByDifferentAuthor = new Explorer.Tours.Core.Domain.Tour(
                "Tour by Author -12",
                "Test description",
                1,
                -12,
                new List<string> { "test" }
            );

            toursContext.Tours.Add(tourByDifferentAuthor);
            toursContext.SaveChanges();

            var createDto = new CouponCreateDto
            {
                DiscountPercentage = 20,
                TourId = tourByDifferentAuthor.Id,
                ValidUntil = DateTime.UtcNow.AddDays(30)
            };

            var actionResult = controller.Create(createDto);

            actionResult.Result.ShouldBeOfType<ForbidResult>();
        }

        [Fact]
        public void Updates_coupon()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateControllerWithRole(scope, "-11", "author");
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            var existingCoupon = dbContext.Coupons.First(c => c.AuthorId == -11);
            var originalCode = existingCoupon.Code;

            var updateDto = new CouponCreateDto
            {
                DiscountPercentage = 30,
                TourId = existingCoupon.TourId,
                ValidUntil = DateTime.UtcNow.AddDays(90)
            };

            // Act
            var actionResult = controller.Update(originalCode, updateDto);
            var result = actionResult.Result.ShouldBeOfType<OkObjectResult>();
            var updated = result.Value.ShouldBeOfType<CouponDto>();

            // Assert
            updated.DiscountPercentage.ShouldBe(30);
            updated.Code.ShouldBe(originalCode);

            var entity = dbContext.Coupons.Find(existingCoupon.Id);
            entity.DiscountPercentage.ShouldBe(30);
        }

        [Fact]
        public void Update_fails_when_not_owner()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateControllerWithRole(scope, "-12", "author");
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            var coupon = dbContext.Coupons.First(c => c.AuthorId == -11);

            var updateDto = new CouponCreateDto
            {
                DiscountPercentage = 50,
                TourId = coupon.TourId,
                ValidUntil = DateTime.UtcNow.AddDays(30)
            };

            // Act & Assert
            var actionResult = controller.Update(coupon.Code, updateDto);
            actionResult.Result.ShouldBeOfType<ForbidResult>();
        }

        [Fact]
        public void Deletes_coupon()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateControllerWithRole(scope, "-11", "author");
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            //var coupon = dbContext.Coupons.First(c => c.AuthorId == -11 && c.Code.StartsWith("TEST"));
            var couponToDelete = new Core.Domain.Coupon(10, -11, null, DateTime.UtcNow.AddDays(5));
            dbContext.Coupons.Add(couponToDelete);
            dbContext.SaveChanges();
            // Odvajamo entitet od contexta da testiramo pravo brisanje
            dbContext.ChangeTracker.Clear();

            // Act
            var result = controller.Delete(couponToDelete.Code);

            // Assert
            result.ShouldBeOfType<NoContentResult>();


            //dbContext.ChangeTracker.Clear();
            //dbContext.Coupons.Find(coupon.Id).ShouldBeNull();
            var deletedCoupon = dbContext.Coupons.FirstOrDefault(c => c.Code == couponToDelete.Code);
            deletedCoupon.ShouldBeNull();

        }

        [Fact]
        public void Delete_fails_when_not_owner()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateControllerWithRole(scope, "-12", "author");
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            var coupon = dbContext.Coupons.First(c => c.AuthorId == -11);

            // Act & Assert
            controller.Delete(coupon.Code).ShouldBeOfType<ForbidResult>();
        }

        private static CouponController CreateControllerWithRole(
            IServiceScope scope,
            string userId,
            string role)
        {
            return new CouponController(
                scope.ServiceProvider.GetRequiredService<ICouponService>(),
                scope.ServiceProvider.GetRequiredService<ITourService>())
            {
                ControllerContext = BuildContextWithRole(userId, role)
            };
        }

        private static ControllerContext BuildContextWithRole(string id, string role)
        {
            var claims = new List<Claim>
            {
                new Claim("id", id),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "test");
            var user = new ClaimsPrincipal(identity);

            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
    }
}