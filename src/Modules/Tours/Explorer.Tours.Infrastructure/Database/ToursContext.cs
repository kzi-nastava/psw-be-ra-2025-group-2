using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Explorer.Tours.Infrastructure.Database;

public class ToursContext : DbContext
{
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<TouristObject> TouristObject { get; set; }
    public DbSet<TouristEquipment> TouristEquipment { get; set; }
    public DbSet<TourProblem> TourProblems { get; set; }

    public DbSet<Tour> Tours { get; set; }
    public DbSet<Monument> Monument { get; set; }

    public DbSet<TourExecution> TourExecutions { get; set; }
    public DbSet<TourReview> TourReviews { get; set; }

    public DbSet<Bundle> Bundles { get; set; }


    public ToursContext(DbContextOptions<ToursContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tours");

        modelBuilder.Entity<TourExecution>()
            .HasMany(te => te.KeyPointVisits)
            .WithOne()
            .HasForeignKey("TourExecutionId")
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Tour>(builder =>
        {
            builder.OwnsMany<KeyPoint>(t => t.KeyPoints, kp =>
            {
                kp.WithOwner().HasForeignKey("TourId");
                kp.Property(k => k.OrdinalNo).IsRequired();
                kp.Property(k => k.Name).IsRequired();
                kp.Property(k => k.Description).IsRequired();
                kp.Property(k => k.SecretText).IsRequired();
                kp.Property(k => k.ImageUrl);
                kp.Property(k => k.Latitude);
                kp.Property(k => k.Longitude);
            });

            builder.OwnsMany(t => t.Durations, duration =>
            {
                duration.WithOwner().HasForeignKey("TourId");
                duration.Property<int>("Id");
                duration.HasKey("Id");
                duration.Property(d => d.TransportType).IsRequired();
                duration.Property(d => d.Minutes).IsRequired();
            });

            builder.Navigation(t => t.KeyPoints)
                    .HasField("_keyPoints")
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(t => t.Reviews)
                   .WithOne()
                   .HasForeignKey(r => r.TourId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(t => t.Reviews)
                   .HasField("_reviews")
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder
                .HasMany(t => t.Equipment)
                .WithMany()
                .UsingEntity(j =>
                {
                    j.ToTable("TourEquipment");
                });
        });

        // NOVA konfiguracija za Bundle
        modelBuilder.Entity<Bundle>(builder =>
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(b => b.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(b => b.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(b => b.AuthorId)
                .IsRequired();

            builder.Property(b => b.TourIds)
                   .HasConversion(
                       v => string.Join(',', v),
                       v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                             .Select(long.Parse)
                             .ToList()
                   )
                   .Metadata.SetValueComparer(
                       new ValueComparer<List<long>>(
                           (c1, c2) => c1.SequenceEqual(c2),
                           c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                           c => c.ToList()
                       )
                   );

            builder.Property(b => b.CreatedAt)
                .IsRequired();

            builder.Property(b => b.UpdatedAt)
                .IsRequired(false);
        });
    }
}