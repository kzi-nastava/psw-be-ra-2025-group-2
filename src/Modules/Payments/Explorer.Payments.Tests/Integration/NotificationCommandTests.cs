using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Explorer.API.Controllers.Administrator.Administration;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Payments.Tests.Integration
{
    [Collection("Sequential")]
    public class NotificationCommandTests : BaseStakeholdersIntegrationTest
    {
        public NotificationCommandTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Tourist_Marks_Notification_As_Read()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateTouristController(scope, "-21");
                var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

                // Proveri da je notifikacija -1 unread
                var notificationBefore = dbContext.Notifications.Find(-1L);
                notificationBefore.ShouldNotBeNull();
                notificationBefore.IsRead.ShouldBeFalse();

                // Act
                var result = controller.MarkAsRead(-1);

                // Assert
                result.ShouldBeOfType<OkObjectResult>();

                var notificationAfter = dbContext.Notifications.Find(-1L);
                notificationAfter.ShouldNotBeNull();
                notificationAfter.IsRead.ShouldBeTrue();
            }
        }

        [Fact]
        public void Mark_As_Read_Fails_For_NonExistent_Notification()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateTouristController(scope, "-21");

                // Act
                var result = controller.MarkAsRead(-9999);

                // Assert
                result.ShouldBeOfType<BadRequestObjectResult>();
            }
        }

        [Fact]
        public void Mark_As_Read_Is_Idempotent()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateTouristController(scope, "-21");
                var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

                // Označi prvi put
                controller.MarkAsRead(-1);

                var notificationAfterFirst = dbContext.Notifications.Find(-1L);
                notificationAfterFirst.IsRead.ShouldBeTrue();

                // Označi drugi put (već je read)
                var result = controller.MarkAsRead(-1);

                // Assert - i dalje treba da uspe
                result.ShouldBeOfType<OkObjectResult>();

                var notificationAfterSecond = dbContext.Notifications.Find(-1L);
                notificationAfterSecond.IsRead.ShouldBeTrue();
            }
        }

        [Fact]
        public void Admin_Deposit_Creates_WalletDeposit_Notification()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var adminController = CreateAdminWalletController(scope);
                var touristController = CreateTouristController(scope, "-22");
                var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

                var initialCount = dbContext.Notifications.Count(n => n.TouristId == -22);

                // Act - Admin deponuje novac
                adminController.AdminDeposit(-22, new WalletController.DepositRequest
                {
                    Amount = 150
                });
                // Assert - Nova notifikacija je kreirana
                var newCount = dbContext.Notifications.Count(n => n.TouristId == -22);
                newCount.ShouldBe(initialCount + 1);

                // Proveri da je notifikacija ispravno kreirana
                var newNotification = dbContext.Notifications
                    .Where(n => n.TouristId == -22)
                    .OrderByDescending(n => n.CreatedAt)
                    .FirstOrDefault();

                newNotification.ShouldNotBeNull();
                newNotification.Message.ShouldContain("150 Adventure Coins");
                newNotification.IsRead.ShouldBeFalse();
                newNotification.Type.ToString().ShouldBe("WalletDeposit");
            }
        }

        [Fact]
        public void Unread_Notifications_Decrease_After_Marking_As_Read()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateTouristController(scope, "-22");

                // Dobavi unread notifikacije pre
                var unreadBefore = controller.GetUnread();
                var unreadListBefore = (unreadBefore as OkObjectResult)?.Value as List<NotificationDto>;
                var countBefore = unreadListBefore?.Count ?? 0;

                // Mark first unread notification as read
                var firstUnread = unreadListBefore?.FirstOrDefault();
                firstUnread.ShouldNotBeNull();

                // Act
                controller.MarkAsRead(firstUnread.Id);

                // Assert
                var unreadAfter = controller.GetUnread();
                var unreadListAfter = (unreadAfter as OkObjectResult)?.Value as List<NotificationDto>;
                var countAfter = unreadListAfter?.Count ?? 0;

                countAfter.ShouldBe(countBefore - 1);
            }
        }

        [Fact]
        public void Tourist_Cannot_See_Other_Tourists_Notifications()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller21 = CreateTouristController(scope, "-21");
                var controller22 = CreateTouristController(scope, "-22");

                // Act
                var result21 = controller21.GetAll();
                var result22 = controller22.GetAll();

                // Assert
                var notifications21 = (result21 as OkObjectResult)?.Value as List<NotificationDto>;
                var notifications22 = (result22 as OkObjectResult)?.Value as List<NotificationDto>;

                // Proveri da turista -21 ne vidi notifikacije turiste -22
                notifications21.ShouldNotContain(n => n.TouristId == -22);
                notifications22.ShouldNotContain(n => n.TouristId == -21);
            }
        }

        private static Explorer.API.Controllers.Tourist.NotificationController CreateTouristController(
            IServiceScope scope,
            string userId)
        {
            return new Explorer.API.Controllers.Tourist.NotificationController(
                scope.ServiceProvider.GetRequiredService<INotificationService>())
            {
                ControllerContext = BuildContext(userId, "tourist")
            };
        }

        private static WalletController CreateAdminWalletController(IServiceScope scope)
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
