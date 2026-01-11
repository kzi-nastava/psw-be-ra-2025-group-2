using Explorer.API.Controllers.Administrator.Administration;
using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Internal;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain.Report;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Integration
{
    [Collection("Sequential")]
    public class TourReportCommandTests : BaseToursIntegrationTest
    {
        public TourReportCommandTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates_report()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-21", "tourist");
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var dto = new CreateTourReportDto
            {
                TourId = -1,
                ReportReason = "Spam content"
            };

            var result = controller.Create(dto).Result.ShouldBeOfType<OkObjectResult>();
            var report = result.Value.ShouldBeOfType<TourReportDto>();

            report.Id.ShouldNotBe(0);
            report.State.ShouldBe("Pending");

            dbContext.TourReports.Any(r => r.Id == report.Id).ShouldBeTrue();
        }

        [Fact]
        public void Accepts_report_and_blocks_author()
        {
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var internalUserServiceMock = scope.ServiceProvider.GetRequiredService<Mock<IInternalUserService>>();
            var controller = CreateAdminController(scope, "-1", "administrator");

            // Arrange: Seed a pending report manually for a tour owned by author -11
            var report = new TourReport(-1, -21, "Offensive language");
            dbContext.TourReports.Add(report);
            dbContext.SaveChanges();

            // Act
            var result = controller.Accept(report.Id);

            // Assert
            result.ShouldBeOfType<OkResult>();

            // Verify cross-module call to block author -11 (from Tour -1)
            internalUserServiceMock.Verify(s => s.BlockUser(-11), Times.Once);

            var updatedReport = dbContext.TourReports.Find(report.Id);
            updatedReport.State.ShouldBe(TourReportState.Accepted);
        }

        [Fact]
        public void Rejects_report()
        {
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var controller = CreateAdminController(scope, "-1", "administrator");

            var report = new TourReport(-1, -21, "Duplicate report");
            dbContext.TourReports.Add(report);
            dbContext.SaveChanges();

            var result = controller.Reject(report.Id);

            result.ShouldBeOfType<OkResult>();
            var updatedReport = dbContext.TourReports.Find(report.Id);
            updatedReport.State.ShouldBe(TourReportState.Rejected);
        }

        private static TourReportController CreateTouristController(IServiceScope scope, string userId, string role)
        {
            return new TourReportController(scope.ServiceProvider.GetRequiredService<ITourReportService>())
            {
                ControllerContext = BuildContextWithRole(userId, role)
            };
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
