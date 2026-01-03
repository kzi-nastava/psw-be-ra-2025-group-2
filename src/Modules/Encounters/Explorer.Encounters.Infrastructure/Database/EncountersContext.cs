using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Infrastructure.Database.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Encounters.Infrastructure.Database
{
    public class EncountersContext : DbContext
    {
        public DbSet<Encounter> Encounters { get; set; }

        public DbSet<EncounterExecution> EncounterExecutions { get; set; }
        public EncountersContext(DbContextOptions<EncountersContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("encounters");

            modelBuilder.ApplyConfiguration(new EncounterConfiguration());

            modelBuilder.Entity<SocialEncounter>().Property(e => e.RequiredPeople).HasColumnName("RequiredPeople");
            modelBuilder.Entity<SocialEncounter>().Property(e => e.Range).HasColumnName("Range");

            modelBuilder.Entity<HiddenLocationEncounter>().Property(e => e.ImageUrl).HasColumnName("ImageUrl");
            modelBuilder.Entity<HiddenLocationEncounter>().Property(e => e.DistanceTreshold).HasColumnName("DistanceTreshold");

            modelBuilder.Entity<HiddenLocationEncounter>().OwnsOne(e => e.ImageLocation, l =>
            {
                l.Property(p => p.Latitude).HasColumnName("ImageLatitude");
                l.Property(p => p.Longitude).HasColumnName("ImageLongitude");
            });

            modelBuilder.Entity<EncounterExecution>()
            .ToTable("EncounterExecutions");
        }
    }
}