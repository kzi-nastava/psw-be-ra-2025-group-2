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
using System.Security.Claims;
using Explorer.Payments.Core.Domain.Wallets;
using Explorer.Stakeholders.API.Public;

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

                long testUserId = -23;

                // 1. PRIPREMA: Osiguraj da wallet za -23 NE POSTOJI ili ga resetuj na 500
                var existingWallet = dbContext.Wallets.FirstOrDefault(w => w.TouristId == testUserId);
                if (existingWallet != null)
                {
                    dbContext.Wallets.Remove(existingWallet);
                    dbContext.SaveChanges();
                }

                // Kreirajmo svež wallet sa 500 balance-om
                var wallet = new Wallet(testUserId);
                wallet.AddAdventureCoins(500);
                dbContext.Wallets.Add(wallet);
                dbContext.SaveChanges();

                // Očisti ChangeTracker da bi sledeći upiti išli direktno u bazu
                dbContext.ChangeTracker.Clear();

                // Act - Dodaj 50
                var result = controller.AdminDeposit(testUserId, new WalletController.DepositRequest { Amount = 50 });

                // Assert
                result.ShouldBeOfType<OkObjectResult>();

                dbContext.ChangeTracker.Clear();

                var updatedWallet = dbContext.Wallets.FirstOrDefault(w => w.TouristId == testUserId);
                updatedWallet.ShouldNotBeNull();
                updatedWallet.Balance.ShouldBe(550); // 500 (početno) + 50 (uplata)
            }
        }

        [Fact]
        public void Admin_Deposit_Creates_Notification()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateAdminController(scope);
                var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
                long testUserId = -22;

                // Priprema: Brišemo stare notifikacije za ovog usera da bi count bio tačan
                var oldNotifs = dbContext.Notifications.Where(n => n.TouristId == testUserId).ToList();
                dbContext.Notifications.RemoveRange(oldNotifs);
                dbContext.SaveChanges();

                var initialNotificationCount = 0; // Sad znamo da je 0

                // Act
                var result = controller.AdminDeposit(testUserId, new WalletController.DepositRequest { Amount = 100 });

                // Assert
                result.ShouldBeOfType<OkObjectResult>();

                var newNotificationCount = dbContext.Notifications.Count(n => n.TouristId == testUserId);
                newNotificationCount.ShouldBe(initialNotificationCount + 1);

                var notification = dbContext.Notifications
                    .Where(n => n.TouristId == testUserId)
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
                var result = controller.AdminDeposit(-21, new WalletController.DepositRequest { Amount = -50 });

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
                var result = controller.AdminDeposit(-21, new WalletController.DepositRequest { Amount = 0 });

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
                long testUserId = -999;

                // 1. PRIPREMA: Osiguraj da wallet ZAISTA ne postoji
                var existingWallet = dbContext.Wallets.FirstOrDefault(w => w.TouristId == testUserId);
                if (existingWallet != null)
                {
                    dbContext.Wallets.Remove(existingWallet);
                    dbContext.SaveChanges();
                    dbContext.ChangeTracker.Clear();
                }

                // Provera pre testa (za svaki slučaj)
                var checkWallet = dbContext.Wallets.FirstOrDefault(w => w.TouristId == testUserId);
                checkWallet.ShouldBeNull();

                // Act
                var result = controller.AdminDeposit(testUserId, new WalletController.DepositRequest { Amount = 200 });

                // Assert
                result.ShouldBeOfType<OkObjectResult>();

                var newWallet = dbContext.Wallets.FirstOrDefault(w => w.TouristId == testUserId);
                newWallet.ShouldNotBeNull();
                newWallet.Balance.ShouldBe(200);
            }
        }

        private static WalletController CreateAdminController(IServiceScope scope)
        {
            return new WalletController(
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