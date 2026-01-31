using Explorer.API.Controllers.Administrator.Administration;
using Explorer.Encounters.API.Dtos.Encounter;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Encounters.Infrastructure.Database;
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

namespace Explorer.Encounters.Tests.Integration.Administration
{
    [Collection("Sequential")]
    public class EncounterCommandTests : BaseEncountersIntegrationTest
    {
        public EncounterCommandTests(EncountersTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates_Social_Encounter()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-1", "administrator");
                var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

                var createDto = new CreateEncounterDto
                {
                    Name = "New social encounter",
                    Description = "Gather people",
                    Latitude = 45.0,
                    Longitude = 19.0,
                    XP = 150,
                    Type = "Social",
                    // Specific fields required for Social
                    RequiredPeople = 5,
                    Range = 15.5
                };

                var result = controller.Create(createDto).Result.ShouldBeOfType<OkObjectResult>();
                var encounterDto = result.Value.ShouldBeOfType<EncounterDto>();

                encounterDto.ShouldNotBeNull();
                encounterDto.Id.ShouldNotBe(0);
                encounterDto.Name.ShouldBe("New social encounter");
                encounterDto.Type.ShouldBe("Social");

                // Check Database directly
                var entity = dbContext.Encounters.Find(encounterDto.Id);
                entity.ShouldNotBeNull();
                entity.ShouldBeOfType<SocialEncounter>();

                var socialEntity = (SocialEncounter)entity;
                socialEntity.RequiredPeople.ShouldBe(5);
                socialEntity.Range.ShouldBe(15.5);
            }
        }

        [Fact]
        public void Creates_Hidden_Location_Encounter()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-1", "administrator");
                var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

                var createDto = new CreateEncounterDto
                {
                    Name = "Find the image",
                    Description = "Look closely",
                    Latitude = 45.0,
                    Longitude = 19.0,
                    XP = 200,
                    Type = "Location",
                    // Specific fields required for Location
                    ImageUrl = "https://test.com/img.jpg",
                    ImageLatitude = 45.1,
                    ImageLongitude = 19.1,
                    DistanceTreshold = 5
                };

                var result = controller.Create(createDto).Result.ShouldBeOfType<OkObjectResult>();
                var encounterDto = result.Value.ShouldBeOfType<EncounterDto>();

                encounterDto.Type.ShouldBe("Location");

                // Check Database
                var entity = dbContext.Encounters.Find(encounterDto.Id);
                entity.ShouldBeOfType<HiddenLocationEncounter>();
                ((HiddenLocationEncounter)entity).ImageUrl.ShouldBe("https://test.com/img.jpg");
            }
        }

        [Fact]
        public void Updates()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-1", "administrator");
                var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

                // We use ID -16 which is a Draft Misc Encounter from SQL script
                var existingId = -16L;

                var updateDto = new UpdateEncounterDto
                {
                    Id = existingId,
                    Name = "Updated name",
                    Description = "Updated description",
                    Latitude = 44.8,
                    Longitude = 20.4,
                    XP = 200,
                    Type = "Miscellaneous"
                };

                var result = controller.Update(updateDto).Result.ShouldBeOfType<OkObjectResult>();
                var updated = result.Value.ShouldBeOfType<EncounterDto>();

                updated.Name.ShouldBe("Updated name");
                updated.Description.ShouldBe("Updated description");
                updated.State.ShouldBe("Draft");

                var entity = dbContext.Encounters.Find(existingId);
                entity.Name.ShouldBe("Updated name");
            }
        }

        [Fact]
        public void Update_fails_when_not_draft()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-1", "administrator");

                // We use ID -1 which is an ACTIVE Social Encounter
                var existingId = -1L;

                var updateDto = new UpdateEncounterDto
                {
                    Id = existingId,
                    Name = "Illegal update",
                    Description = "Should fail",
                    Latitude = 45,
                    Longitude = 19,
                    XP = 100,
                    Type = "Social"
                };

                controller.Update(updateDto).Result.ShouldBeOfType<BadRequestObjectResult>();
            }
        }

        [Fact]
        public void Activates()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-1", "administrator");
                var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

                // ID -17 is a Draft Location Encounter
                var encounterId = -17L;

                controller.Activate(encounterId).ShouldBeOfType<OkResult>();

                var entity = dbContext.Encounters.Find(encounterId);
                entity.State.ShouldBe(EncounterState.Active);
            }
        }

        [Fact]
        public void Archives()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-1", "administrator");
                var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

                // ID -1 is an Active Social Encounter
                var encounterId = -1L;

                controller.Archive(encounterId).ShouldBeOfType<OkResult>();

                var entity = dbContext.Encounters.Find(encounterId);
                entity.State.ShouldBe(EncounterState.Archived);
            }
        }

        [Fact]
        public void Deletes()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-1", "administrator");
                var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();

                // ID -2 is an Active Location Encounter
                var encounterId = -2L;

                controller.Delete(encounterId).ShouldBeOfType<NoContentResult>();

                dbContext.Encounters.Find(encounterId).ShouldBeNull();
            }
        }

        [Fact]
        public void Tourist_can_get_active_only()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var controller = CreateControllerWithRole(scope, "-21", "tourist");

                var result = controller.GetActive().Result.ShouldBeOfType<OkObjectResult>();
                var encounters = result.Value.ShouldBeOfType<List<EncounterDto>>();

                // Check that we retrieved active encounters
                encounters.ShouldNotBeEmpty();
                encounters.All(e => e.State == "Active").ShouldBeTrue();
            }
        }

        [Fact]
        public void FULL_DIAGNOSIS_Test()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();
                var repo = scope.ServiceProvider.GetRequiredService<IEncounterRepository>();

                Console.WriteLine("========== FULL DIAGNOSIS ==========\n");

                // 1. Proveri koliko ima encounters
                var allEncounters = dbContext.Encounters.ToList();
                Console.WriteLine($"Total Encounters: {allEncounters.Count}");

                // 2. Proveri SPECIFIČNE ID-jeve koje testovi koriste
                Console.WriteLine("\n--- Testing specific IDs that FAIL ---");

                // Test Update (koristi -16)
                var enc16 = repo.GetById(-16);
                Console.WriteLine($"ID -16 (Draft Misc for Update test): {(enc16 != null ? $"FOUND - {enc16.Name}, State={enc16.State}" : "NULL")}");

                // Test Update_fails (koristi -1)
                var enc1 = repo.GetById(-1);
                Console.WriteLine($"ID -1 (Active Social for Update_fails test): {(enc1 != null ? $"FOUND - {enc1.Name}, State={enc1.State}" : "NULL")}");

                // Test Activate (koristi -17)
                var enc17 = repo.GetById(-17);
                Console.WriteLine($"ID -17 (Draft Location for Activate test): {(enc17 != null ? $"FOUND - {enc17.Name}, State={enc17.State}" : "NULL")}");

                // Test Archive (koristi -1)
                Console.WriteLine($"ID -1 already checked above");

                // Test Delete (koristi -2)
                var enc2 = repo.GetById(-2);
                Console.WriteLine($"ID -2 (Active Location for Delete test): {(enc2 != null ? $"FOUND - {enc2.Name}, State={enc2.State}" : "NULL")}");

                // 3. Ispiši SVE encounters da vidimo šta postoji
                Console.WriteLine("\n--- ALL Encounters in DB ---");
                foreach (var e in allEncounters.OrderBy(x => x.Id))
                {
                    Console.WriteLine($"  ID={e.Id}, Name={e.Name}, State={e.State}, Type={e.Type}");
                }

                // 4. Proveri da li postoje DUPLIKATI
                var duplicates = allEncounters.GroupBy(e => e.Id).Where(g => g.Count() > 1);
                if (duplicates.Any())
                {
                    Console.WriteLine("\n❌ FOUND DUPLICATES:");
                    foreach (var dup in duplicates)
                    {
                        Console.WriteLine($"  ID {dup.Key} appears {dup.Count()} times!");
                    }
                }
            }
        }

        private static EncounterController CreateControllerWithRole(
            IServiceScope scope,
            string userId,
            string role)
        {
            return new EncounterController(
                scope.ServiceProvider.GetRequiredService<IEncounterService>(),
                scope.ServiceProvider.GetRequiredService<IRewardService>())
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