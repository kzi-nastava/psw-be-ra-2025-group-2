using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.API.Controllers.Administrator.Administration;
using Explorer.Payments.API.Public;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using System.Security.Claims;
using Explorer.Payments.Core.Domain.Wallets;

namespace Explorer.Payments.Tests.Integration
{
    [Collection("Sequential")]
    public class WalletCommandTests : BaseStakeholdersIntegrationTest
    {
        public WalletCommandTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Admin_Deposits_Success()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateAdminController(scope);
                var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

                // Act
                var result = controller.AdminDeposit(-21, 50);

                // Assert
                result.ShouldBeOfType<OkObjectResult>();

                // Refresh podatke iz baze
                dbContext.ChangeTracker.Clear();

                var updatedWallet = dbContext.Wallets.FirstOrDefault(w => w.TouristId == -21);
                updatedWallet.ShouldNotBeNull();
                updatedWallet.Balance.ShouldBe(150); // 100 (initial) + 50 (deposited)
            }
        }

        [Fact]
        public void Admin_Deposit_Creates_Notification()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateAdminController(scope);
                var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

                var initialNotificationCount = dbContext.Notifications.Count(n => n.TouristId == -22);

                // Act
                var result = controller.AdminDeposit(-22, 100);

                // Assert
                result.ShouldBeOfType<OkObjectResult>();

                var newNotificationCount = dbContext.Notifications.Count(n => n.TouristId == -22);
                newNotificationCount.ShouldBe(initialNotificationCount + 1);

                var notification = dbContext.Notifications
                    .Where(n => n.TouristId == -22)
                    .OrderByDescending(n => n.CreatedAt)
                    .FirstOrDefault();

                notification.ShouldNotBeNull();
                notification.Message.ShouldContain("100 Adventure Coins");
                notification.IsRead.ShouldBeFalse();
            }
        }

        [Fact]
        public void Admin_Deposit_Fails_Negative_Amount()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateAdminController(scope);

                // Act
                var result = controller.AdminDeposit(-21, -50);

                // Assert
                result.ShouldBeOfType<BadRequestObjectResult>();
            }
        }

        [Fact]
        public void Admin_Deposit_Fails_Zero_Amount()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateAdminController(scope);

                // Act
                var result = controller.AdminDeposit(-21, 0);

                // Assert
                result.ShouldBeOfType<BadRequestObjectResult>();
            }
        }

        [Fact]
        public void Admin_Deposit_Creates_Wallet_If_Not_Exists()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateAdminController(scope);
                var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

                // Ensure wallet doesn't exist
                var existingWallet = dbContext.Wallets.FirstOrDefault(w => w.TouristId == -999);
                existingWallet.ShouldBeNull();

                // Act
                var result = controller.AdminDeposit(-999, 200);

                // Assert
                result.ShouldBeOfType<OkObjectResult>();

                var newWallet = dbContext.Wallets.FirstOrDefault(w => w.TouristId == -999);
                newWallet.ShouldNotBeNull();
                newWallet.Balance.ShouldBe(200);
            }
        }

        private static Explorer.API.Controllers.Administrator.Administration.WalletController CreateAdminController(IServiceScope scope)
        {
            return new Explorer.API.Controllers.Administrator.Administration.WalletController(
                scope.ServiceProvider.GetRequiredService<IWalletService>())
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
