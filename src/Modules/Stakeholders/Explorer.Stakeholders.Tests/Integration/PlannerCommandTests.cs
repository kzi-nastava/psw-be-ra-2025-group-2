using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos.Planner;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.Exceptions;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class PlannerCommandTests : BaseStakeholdersIntegrationTest
    {
        public PlannerCommandTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates_schedule_entry_and_day_entry()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-21", "tourist");
                var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

                var newScheduleDto = new CreateScheduleDto
                {
                    Date = new DateOnly(2026, 5, 20),
                    TourId = -1, // Belgrade Historical Walk (Owned by -21)
                    DayNotes = "A busy day in Belgrade",
                    Notes = "Morning session",
                    Start = new DateTime(2026, 5, 20, 9, 0, 0, DateTimeKind.Utc),
                    End = new DateTime(2026, 5, 20, 11, 0, 0, DateTimeKind.Utc)
                };

                var result = controller.CreateScheduleEntry(newScheduleDto).Result.ShouldBeOfType<OkObjectResult>();
                var dayEntryDto = result.Value.ShouldBeOfType<DayEntryDto>();

                dayEntryDto.ShouldNotBeNull();
                dayEntryDto.TouristId.ShouldBe(-21);
                dayEntryDto.Date.ShouldBe(new DateOnly(2026, 5, 20));
                dayEntryDto.Entries.Count().ShouldBe(1);
                dayEntryDto.Entries.First().TourName.ShouldBe("Belgrade Historical Walk");

                // Verify Database
                var storedEntity = dbContext.PlannerDayEntries.Include(x => x.Entries).FirstOrDefault(x => x.Id == dayEntryDto.Id);
                storedEntity.ShouldNotBeNull();
                storedEntity.Notes.ShouldBe("A busy day in Belgrade");
                storedEntity.Entries.Count.ShouldBe(1);
            }
        }

        [Fact]
        public void Create_fails_nonexistent_tour()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-21", "tourist");

                var newScheduleDto = new CreateScheduleDto
                {
                    Date = new DateOnly(2026, 5, 20),
                    TourId = 9999, // ID that does not exist in seed data
                    DayNotes = "Exploring",
                    Start = new DateTime(2026, 5, 20, 10, 0, 0, DateTimeKind.Utc),
                    End = new DateTime(2026, 5, 20, 11, 0, 0, DateTimeKind.Utc)
                };

                // Act & Assert
                Should.Throw<TourNotOwnedException>(() => controller.CreateScheduleEntry(newScheduleDto));
            }
        }

        [Fact]
        public void Fails_to_create_schedule_for_unowned_tour()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-21", "tourist");

                var newScheduleDto = new CreateScheduleDto
                {
                    Date = new DateOnly(2026, 5, 20),
                    TourId = -10, // Owned by -22, not -21
                    Start = new DateTime(2026, 5, 20, 10, 0, 0, DateTimeKind.Utc),
                    End = new DateTime(2026, 5, 20, 12, 0, 0, DateTimeKind.Utc)
                };

                // Assuming ownership checker throws ForbiddenException or similar handled by middleware
                Should.Throw<Exception>(() => controller.CreateScheduleEntry(newScheduleDto));
            }
        }

        [Fact]
        public void Updates_day_notes()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-21", "tourist");
                var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

                // Create initial
                var setupResult = controller.CreateScheduleEntry(new CreateScheduleDto
                {
                    Date = new DateOnly(2026, 6, 1),
                    TourId = -1,
                    DayNotes = "Old Note",
                    Start = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc),
                    End = new DateTime(2026, 6, 1, 11, 0, 0, DateTimeKind.Utc)
                }).Result.ShouldBeOfType<OkObjectResult>();
                var initialDto = setupResult.Value.ShouldBeOfType<DayEntryDto>();

                var updateDto = new UpdateDayNotesDto { Id = initialDto.Id, Notes = "Updated Day Note" };

                var result = controller.UpdateDayNotes(updateDto).Result.ShouldBeOfType<OkObjectResult>();
                var updatedDto = result.Value.ShouldBeOfType<DayEntryDto>();

                updatedDto.Notes.ShouldBe("Updated Day Note");
                dbContext.PlannerDayEntries.Find(initialDto.Id).Notes.ShouldBe("Updated Day Note");
            }
        }

        [Fact]
        public void Deletes_schedule_entry()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-21", "tourist");
                var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

                // Setup
                var setup = controller.CreateScheduleEntry(new CreateScheduleDto
                {
                    Date = new DateOnly(2026, 7, 1),
                    TourId = -1,
                    Start = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc),
                    End = new DateTime(2026, 7, 1, 11, 0, 0, DateTimeKind.Utc)
                }).Result.ShouldBeOfType<OkObjectResult>();

                var dayDto = setup.Value.ShouldBeOfType<DayEntryDto>();
                var scheduleId = dayDto.Entries.First().Id;

                // Act
                controller.RemoveScheduleEntry(scheduleId).ShouldBeOfType<OkResult>();

                // Assert
                var entity = dbContext.PlannerDayEntries.Include(x => x.Entries).First(x => x.Id == dayDto.Id);
                entity.Entries.ShouldBeEmpty();
            }
        }

        [Fact]
        public void Delete_fails_nonexistent_schedule_entry()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-21", "tourist");
                long nonExistentScheduleId = 8888;

                // Act & Assert
                // This targets the _plannerRepository.GetByScheduleEntryId(id) logic
                Should.Throw<NotFoundException>(() => controller.RemoveScheduleEntry(nonExistentScheduleId))
                      .Message.ShouldBe("Day entry not found.");
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
            var claims = new List<Claim>
        {
            new Claim("id", id),
            new Claim(ClaimTypes.Role, role)
        };
            var identity = new ClaimsIdentity(claims, "test");
            return new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) } };
        }
    }
}
