using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Infrastructure.Database.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Encounters.Infrastructure.Database
{
    public class EncountersContext : DbContext
    {
        public DbSet<Encounter> Encounters { get; set; }
        public DbSet<EncounterExecution> EncounterExecutions { get; set; }
        public DbSet<TouristProgress> TouristProgresses { get; set; }
        public DbSet<EncounterPresence> EncounterPresences { get; set; }
        public EncountersContext(DbContextOptions<EncountersContext> options) : base(options) { }
        public DbSet<ProfileFrame> ProfileFrames { get; set; }
        public DbSet<TouristUnlockedFrame> TouristUnlockedFrames { get; set; }
        public DbSet<TouristProfileFrameSettings> TouristProfileFrameSettings { get; set; }


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

            modelBuilder.Entity<EncounterExecution>(b =>
            {
                b.ToTable("EncounterExecutions");

                // Unique per user+encounter (prevents duplicates)
                b.HasIndex(e => new { e.UserId, e.EncounterId }).IsUnique();

                b.Property(x => x.StartedAt).IsRequired();
                b.Property(x => x.LastPingAt).IsRequired(false);
                b.Property(x => x.SecondsInsideZone).IsRequired();

                b.Property(x => x.CompletionTime).IsRequired(false);
                b.Property(x => x.IsCompleted).IsRequired();

                b.Property(x => x.XpAwarded).IsRequired();
            });

            modelBuilder.Entity<TouristProgress>()
                .ToTable("TouristProgresses");
            
            modelBuilder.Entity<ProfileFrame>(b =>
            {
                b.ToTable("ProfileFrames");
                b.Property(x => x.Name).IsRequired().HasMaxLength(100);
                b.Property(x => x.AssetKey).IsRequired().HasMaxLength(200);
                b.HasIndex(x => x.LevelRequirement).IsUnique();
            });

            modelBuilder.Entity<TouristUnlockedFrame>(b =>
            {
                b.ToTable("TouristUnlockedFrames");
                b.Property(x => x.UnlockedAt).IsRequired();
                b.HasIndex(x => new { x.UserId, x.FrameId }).IsUnique();
            });

            modelBuilder.Entity<TouristProfileFrameSettings>(b =>
            {
                b.ToTable("TouristProfileFrameSettings");
                b.Property(x => x.ShowFramesEnabled).IsRequired();
                b.HasIndex(x => x.UserId).IsUnique();
            });

        }
    }
}
