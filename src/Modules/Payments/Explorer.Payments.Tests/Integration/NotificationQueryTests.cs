using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Payments.Tests.Integration
{
    [Collection("Sequential")]
    public class NotificationQueryTests : BaseStakeholdersIntegrationTest
    {
        public NotificationQueryTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Tourist_Gets_All_Notifications()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateTouristController(scope, "-21");

                // Act
                var result = controller.GetAll();

                // Assert
                var okResult = result.ShouldBeOfType<OkObjectResult>();
                var notifications = okResult.Value.ShouldBeOfType<List<NotificationDto>>();

                notifications.Count.ShouldBe(3); // Tourist -21 ima 3 notifikacije
                notifications.All(n => n.TouristId == -21).ShouldBeTrue();
            }
        }

        [Fact]
        public void Tourist_Gets_Only_Unread_Notifications()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateTouristController(scope, "-21");

                // Act
                var result = controller.GetUnread();

                // Assert
                var okResult = result.ShouldBeOfType<OkObjectResult>();
                var notifications = okResult.Value.ShouldBeOfType<List<NotificationDto>>();

                notifications.Count.ShouldBe(2); // Tourist -21 ima 2 unread notifikacije
                notifications.All(n => !n.IsRead).ShouldBeTrue();
                notifications.All(n => n.TouristId == -21).ShouldBeTrue();
            }
        }

        [Fact]
        public void Tourist_Gets_Empty_List_When_No_Notifications()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                // Tourist -999 nema notifikacije u seed podacima
                var controller = CreateTouristController(scope, "-999");

                // Act
                var result = controller.GetAll();

                // Assert
                var okResult = result.ShouldBeOfType<OkObjectResult>();
                var notifications = okResult.Value.ShouldBeOfType<List<NotificationDto>>();

                notifications.ShouldBeEmpty();
            }
        }

        [Fact]
        public void Tourist_Gets_Notifications_Ordered_By_Date()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateTouristController(scope, "-21");

                // Act
                var result = controller.GetAll();

                // Assert
                var okResult = result.ShouldBeOfType<OkObjectResult>();
                var notifications = okResult.Value.ShouldBeOfType<List<NotificationDto>>();

                // Proveri da su sortirane od najnovije ka najstarijoj
                for (int i = 0; i < notifications.Count - 1; i++)
                {
                    notifications[i].CreatedAt.ShouldBeGreaterThanOrEqualTo(notifications[i + 1].CreatedAt);
                }
            }
        }

        [Fact]
        public void Different_Tourists_Get_Their_Own_Notifications()
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

                notifications21.ShouldNotBeNull();
                notifications22.ShouldNotBeNull();

                notifications21.Count.ShouldBe(3);
                notifications22.Count.ShouldBe(2);

                notifications21.All(n => n.TouristId == -21).ShouldBeTrue();
                notifications22.All(n => n.TouristId == -22).ShouldBeTrue();
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
