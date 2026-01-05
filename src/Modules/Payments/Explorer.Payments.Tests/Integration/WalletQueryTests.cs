using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.API.Public;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Payments.Tests.Integration
{
    [Collection("Sequential")]
    public class WalletQueryTests : BaseStakeholdersIntegrationTest
    {
        public WalletQueryTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Tourist_Gets_Balance_Success()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateTouristController(scope, "-21");

                // Act
                var result = controller.GetBalance();

                // Assert - ActionResult<int> vraća Result property
                var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
                var balance = (int)okResult.Value;
                balance.ShouldBe(100);
            }
        }

        [Fact]
        public void Tourist_Gets_Zero_Balance_For_New_Wallet()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateTouristController(scope, "-999");

                // Act
                var result = controller.GetBalance();

                // Assert
                var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
                var balance = (int)okResult.Value;
                balance.ShouldBe(0);
            }
        }

        [Fact]
        public void Tourist_Gets_Balance_After_Deposit()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var touristController = CreateTouristController(scope, "-22");
                var adminController = CreateAdminController(scope);

                // Admin deposits
                adminController.AdminDeposit(-22,new Explorer.API.Controllers.Administrator.Administration.WalletController.DepositRequest
                    {
                        Amount = 150
                    }
                );

                // Tourist checks balance
                var result = touristController.GetBalance();

                // Assert
                var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
                var balance = (int)okResult.Value;
                balance.ShouldBe(150); // 0 (initial) + 150 (deposited)
            }
        }

        private static Explorer.API.Controllers.Tourist.WalletController CreateTouristController(IServiceScope scope, string userId)
        {
            return new Explorer.API.Controllers.Tourist.WalletController(
                scope.ServiceProvider.GetRequiredService<IWalletService>())
            {
                ControllerContext = BuildContext(userId, "tourist")
            };
        }

        private static Explorer.API.Controllers.Administrator.Administration.WalletController CreateAdminController(IServiceScope scope)
        {
            return new Explorer.API.Controllers.Administrator.Administration.WalletController(
                scope.ServiceProvider.GetRequiredService<IWalletService>(),
                scope.ServiceProvider.GetRequiredService<IUserService>()
            )
            {
                ControllerContext = BuildContext("-1", "administrator")
            };
        }


        private static ControllerContext BuildContext(string id, string role)
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
