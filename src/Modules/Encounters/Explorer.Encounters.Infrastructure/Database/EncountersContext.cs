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
        public DbSet<UserReward> UserRewards { get; set; }

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

            modelBuilder.Entity<UserReward>(b =>
            {
                b.ToTable("UserRewards");
                b.HasKey(r => r.Id);
                b.Property(r => r.UserId).IsRequired();
                b.Property(r => r.Level).IsRequired();
                b.Property(r => r.CouponCode).IsRequired();
                b.Property(r => r.DiscountPercentage).IsRequired();
                b.Property(r => r.GrantedAt).IsRequired();
                b.Property(r => r.ValidUntil);
                b.Property(r => r.IsUsed).IsRequired();
                b.Property(r => r.Description).IsRequired();
            });

            modelBuilder.Entity<TouristProgress>()
                .ToTable("TouristProgresses");
        }
    }
}
