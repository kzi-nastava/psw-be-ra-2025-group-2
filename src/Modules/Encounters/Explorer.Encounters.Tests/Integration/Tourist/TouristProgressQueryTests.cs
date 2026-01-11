using Explorer.Encounters.API.Dtos.Encounter;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Encounters.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class TouristProgressQueryTests : BaseEncountersIntegrationTest
    {
        public TouristProgressQueryTests(EncountersTestFactory factory) : base(factory) { }

        [Fact]
        public void Complete_creates_and_updates_TouristProgress_in_database()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();
            var db = scope.ServiceProvider.GetRequiredService<EncountersContext>();

            var touristId = -21L;

            var existing = db.Set<TouristProgress>().FirstOrDefault(p => p.UserId == touristId);
            if (existing != null)
            {
                db.Remove(existing);
                db.SaveChanges();
            }

            var encounterId = CreateAndActivateMiscEncounter(service, xp: 150);

            service.CompleteEncounter(touristId, encounterId);

            var progress = db.Set<TouristProgress>()
                .FirstOrDefault(p => p.UserId == touristId);

            progress.ShouldNotBeNull();
            progress!.UserId.ShouldBe(touristId);
            progress.TotalXp.ShouldBe(150);
            progress.Level.ShouldBe(2); 
            progress.CanCreateChallenges.ShouldBeFalse();
        }

        private static long CreateAndActivateMiscEncounter(IEncounterService service, int xp)
        {
            var created = service.Create(new CreateEncounterDto
            {
                Name = "Query test encounter",
                Description = "test",
                Latitude = 45.0,
                Longitude = 19.0,
                XP = xp,
                Type = "Miscellaneous"
            });

            service.MakeActive(created.Id);
            return created.Id;
        }
    }
}
