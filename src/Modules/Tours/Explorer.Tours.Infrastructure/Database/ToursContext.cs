using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.Execution;
using Explorer.Tours.Core.Domain.Report;
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

    public DbSet<PublicKeyPoint> PublicKeyPoints { get; set; }
    public DbSet<PublicKeyPointRequest> PublicKeyPointRequests { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    public DbSet<TourExecution> TourExecutions { get; set; }
    public DbSet<TourReview> TourReviews { get; set; }
    public DbSet<TourReport> TourReports { get; set; }

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
            builder.OwnsMany(t => t.KeyPoints, kp =>
            {
                kp.WithOwner().HasForeignKey("TourId");
                kp.Property(k => k.OrdinalNo).IsRequired();
                kp.Property(k => k.Name).IsRequired();
                kp.Property(k => k.Description).IsRequired();
                kp.Property(k => k.SecretText).IsRequired();
                kp.Property(k => k.ImageUrl);

                kp.Property(k => k.Latitude)
                    .HasColumnType("double precision")
                    .HasConversion(v => v, v => v)
                    .IsRequired();

                kp.Property(k => k.Longitude)
                    .HasColumnType("double precision")
                    .HasConversion(v => v, v => v)
                    .IsRequired();

                kp.Property(k => k.AuthorId).IsRequired();
                kp.Property(k => k.IsPublic).IsRequired();
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

            builder.HasMany(t => t.Equipment)
                   .WithMany()
                   .UsingEntity(j => { j.ToTable("TourEquipment"); });

            
            
            // EnvironmentType - nullable enum stored as int
            builder.Property(t => t.EnvironmentType)
                   .HasConversion<int?>()
                   .IsRequired(false);

            // FoodTypes - List<FoodType> stored as comma-separated string
            builder.Property(t => t.FoodTypes)
                   .HasConversion(
                       v => string.Join(',', v.Select(ft => (int)ft)),
                       v => string.IsNullOrEmpty(v) 
                           ? new List<FoodType>() 
                           : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(x => (FoodType)int.Parse(x))
                              .ToList()
                   )
                   .Metadata.SetValueComparer(
                       new ValueComparer<List<FoodType>>(
                           (c1, c2) => c1.SequenceEqual(c2),
                           c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                           c => c.ToList()
                       )
                   );

            // AdventureLevel - nullable enum stored as int
            builder.Property(t => t.AdventureLevel)
                   .HasConversion<int?>()
                   .IsRequired(false);

            // ActivityTypes - List<ActivityType> stored as comma-separated string
            builder.Property(t => t.ActivityTypes)
                   .HasConversion(
                       v => string.Join(',', v.Select(at => (int)at)),
                       v => string.IsNullOrEmpty(v) 
                           ? new List<ActivityType>() 
                           : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(x => (ActivityType)int.Parse(x))
                              .ToList()
                   )
                   .Metadata.SetValueComparer(
                       new ValueComparer<List<ActivityType>>(
                           (c1, c2) => c1.SequenceEqual(c2),
                           c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                           c => c.ToList()
                       )
                   );

            // SuitableForGroups - List<SuitableFor> stored as comma-separated string
            builder.Property(t => t.SuitableForGroups)
                   .HasConversion(
                       v => string.Join(',', v.Select(sf => (int)sf)),
                       v => string.IsNullOrEmpty(v) 
                           ? new List<SuitableFor>() 
                           : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(x => (SuitableFor)int.Parse(x))
                              .ToList()
                   )
                   .Metadata.SetValueComparer(
                       new ValueComparer<List<SuitableFor>>(
                           (c1, c2) => c1.SequenceEqual(c2),
                           c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                           c => c.ToList()
                       )
                   );
           
        });

        modelBuilder.Entity<PublicKeyPoint>(entity =>
        {
            entity.ToTable("PublicKeyPoints");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.SecretText).HasMaxLength(1000);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);

            entity.Property(e => e.Latitude)
                .HasColumnType("double precision")
                .HasConversion(v => v, v => v)
                .IsRequired();

            entity.Property(e => e.Longitude)
                .HasColumnType("double precision")
                .HasConversion(v => v, v => v)
                .IsRequired();

            entity.Property(e => e.AuthorId).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.SourceTourId);
            entity.Property(e => e.SourceOrdinalNo);

            entity.HasIndex(e => e.AuthorId);
        });

        modelBuilder.Entity<PublicKeyPointRequest>(entity =>
        {
            entity.ToTable("PublicKeyPointRequests");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PublicKeyPointId).IsRequired();
            entity.Property(e => e.AuthorId).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasConversion<string>();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.RejectionReason).HasMaxLength(500);

            entity.HasOne(e => e.PublicKeyPoint)
                  .WithMany()
                  .HasForeignKey(e => e.PublicKeyPointId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.AuthorId);
            entity.HasIndex(e => e.Status);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notifications");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Type).IsRequired().HasConversion<string>();
            entity.Property(e => e.IsRead).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => e.UserId);
        });

        modelBuilder.Entity<TourReport>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.TourId).IsRequired();
            entity.Property(r => r.TouristId).IsRequired();
            entity.Property(r => r.ReportReason).IsRequired();

            entity.Property(r => r.State).IsRequired().HasConversion<string>();
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