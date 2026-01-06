using Explorer.API.Controllers.Administrator.Administration;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Integration
{
    public class TourReportQueryTests : BaseToursIntegrationTest
    {
        public TourReportQueryTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Gets_pending_reports_for_admin()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateAdminController(scope, "-1", "administrator");

                var result = controller.GetPendingReports().Result.ShouldBeOfType<OkObjectResult>();
                var reports = result.Value.ShouldBeOfType<List<TourReportDto>>();

                // Assert based on your seeded SQL data
                reports.ForEach(r => r.State.ShouldBe("Pending"));
            }
        }

        private static TourReportAdministrationController CreateAdminController(IServiceScope scope, string userId, string role)
        {
            return new TourReportAdministrationController(scope.ServiceProvider.GetRequiredService<ITourReportAdministrationService>())
            {
                ControllerContext = BuildContextWithRole(userId, role)
            };
        }

        private static ControllerContext BuildContextWithRole(string id, string role)
        {
            var claims = new List<Claim> { new Claim("id", id), new Claim(ClaimTypes.Role, role) };
            var identity = new ClaimsIdentity(claims, "test");
            return new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) } };
        }
    }
}
