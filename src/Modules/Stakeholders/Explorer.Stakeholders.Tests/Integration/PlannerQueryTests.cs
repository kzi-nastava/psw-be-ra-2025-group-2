using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos.Planner;
using Explorer.Stakeholders.API.Public;
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

namespace Explorer.Stakeholders.Tests.Integration
{
    [Collection("Sequential")]
    public class PlannerQueryTests : BaseStakeholdersIntegrationTest
    {
        public PlannerQueryTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Gets_monthly_schedule_with_tour_names()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-21", "tourist");

                // Seed a couple of entries for May 2026
                controller.CreateScheduleEntry(new CreateScheduleDto
                {
                    Date = new DateOnly(2026, 5, 10),
                    TourId = -1,
                    Start = new DateTime(2026, 5, 10, 10, 0, 0, DateTimeKind.Utc),
                    End = new DateTime(2026, 5, 10, 12, 0, 0, DateTimeKind.Utc)
                });

                controller.CreateScheduleEntry(new CreateScheduleDto
                {
                    Date = new DateOnly(2026, 5, 15),
                    TourId = -2,
                    Start = new DateTime(2026, 5, 15, 08, 0, 0, DateTimeKind.Utc),
                    End = new DateTime(2026, 5, 15, 10, 0, 0, DateTimeKind.Utc)
                });

                // Act
                var result = controller.GetMonthlySchedule(2026, 5).Result.ShouldBeOfType<OkObjectResult>();
                var schedule = result.Value.ShouldBeOfType<List<DayEntryDto>>();

                // Assert
                schedule.Count().ShouldBe(2);
                var firstDay = schedule.First(d => d.Date == new DateOnly(2026, 5, 10));
                firstDay.Entries.First().TourName.ShouldBe("Belgrade Historical Walk");

                var secondDay = schedule.First(d => d.Date == new DateOnly(2026, 5, 15));
                secondDay.Entries.First().TourName.ShouldBe("Tara National Park Adventure");
            }
        }

        [Fact]
        public void Returns_empty_list_for_month_with_no_entries()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-21", "tourist");

                var result = controller.GetMonthlySchedule(2029, 12).Result.ShouldBeOfType<OkObjectResult>();
                var schedule = result.Value.ShouldBeOfType<List<DayEntryDto>>();

                schedule.ShouldBeEmpty();
            }
        }

        [Fact]
        public void Gets_monthly_schedule_for_tourist()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-21", "tourist");

                // Tourist -21 has 2 DayEntries in February 2026
                var result = controller.GetMonthlySchedule(2026, 2).Result.ShouldBeOfType<OkObjectResult>();
                var schedule = result.Value.ShouldBeOfType<List<DayEntryDto>>();

                schedule.Count.ShouldBe(2);
                schedule.Any(d => d.Date == new DateOnly(2026, 2, 13)).ShouldBeTrue();

                // Check enrichment: Tour -1 name should be "Belgrade Historical Walk"
                var day13 = schedule.First(d => d.Date == new DateOnly(2026, 2, 13));
                day13.Entries.Any(e => e.TourName == "Belgrade Historical Walk").ShouldBeTrue();
            }
        }

        [Fact]
        public void Gets_suggestions_for_overbooked_day()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-21", "tourist");

                // Tourist -21 has 4 tours on Feb 13th
                var result = controller.GetSuggestions(2026, 2, 13).Result.ShouldBeOfType<OkObjectResult>();
                var suggestions = result.Value.ShouldBeOfType<List<SuggestionDto>>();

                suggestions.ShouldNotBeEmpty();
                suggestions.Any(s => s.Kind == "OverbookedSchedule").ShouldBeTrue();
                suggestions.Any(s => s.Kind == "SmallTimeSlot").ShouldBeTrue();
                suggestions.Select(s => s.Date).ShouldBeInOrder();
            }
        }

        [Fact]
        public void Gets_suggestions_for_valid_day()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-22", "tourist");

                // Tourist -22 has a standard 2-tour day on Feb 13th
                var result = controller.GetSuggestions(2026, 2, 13).Result.ShouldBeOfType<OkObjectResult>();
                var suggestions = result.Value.ShouldBeOfType<List<SuggestionDto>>();

                suggestions.ShouldBeEmpty();
            }
        }

        [Fact]
        public void Gets_suggestions_for_entire_month()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-21", "tourist");

                // day is null -> Monthly scope
                var result = controller.GetSuggestions(2026, 2, null).Result.ShouldBeOfType<OkObjectResult>();
                var suggestions = result.Value.ShouldBeOfType<List<SuggestionDto>>();

                // Should aggregate issues across the month
                suggestions.ShouldNotBeEmpty();
                suggestions.Count(s => s.Kind == "OverbookedSchedule").ShouldBe(1);
            }
        }

        private static PlannerController CreateControllerWithRole(IServiceScope scope, string userId, string role)
        {
            return new PlannerController(scope.ServiceProvider.GetRequiredService<IPlannerService>())
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
