using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Explorer.API.Controllers.Author;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
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
    public class CouponQueryTests : BaseStakeholdersIntegrationTest
    {
        public CouponQueryTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void GetByCode_returns_coupon()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateControllerWithRole(scope, "2", "author");
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            var existingCoupon = dbContext.Coupons.First();

            // Act
            var result = controller.GetByCode(existingCoupon.Code).Result.ShouldBeOfType<OkObjectResult>();
            var coupon = result.Value.ShouldBeOfType<CouponDto>();

            // Assert
            coupon.Code.ShouldBe(existingCoupon.Code);
            coupon.DiscountPercentage.ShouldBe(existingCoupon.DiscountPercentage);
        }

        [Fact]
        public void GetByCode_returns_not_found_for_invalid_code()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateControllerWithRole(scope, "2", "author");

            // Act & Assert
            controller.GetByCode("INVALID123").Result.ShouldBeOfType<NotFoundObjectResult>();
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
