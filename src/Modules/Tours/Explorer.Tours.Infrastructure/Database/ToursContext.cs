using Explorer.Tours.Core.Domain;
using Microsoft.EntityFrameworkCore;

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

    public ToursContext(DbContextOptions<ToursContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tours");

        modelBuilder.Entity<Tour>(entity =>
        {
            entity.OwnsMany<KeyPoint>(t => t.KeyPoints, kp =>
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
                kp.Property(k => k.PublicStatus).IsRequired().HasConversion<string>();
            });

            entity.OwnsMany(t => t.Durations, duration =>
            {
                duration.WithOwner().HasForeignKey("TourId");
                duration.Property<int>("Id");
                duration.HasKey("Id");
                duration.Property(d => d.TransportType).IsRequired();
                duration.Property(d => d.Minutes).IsRequired();
            });

            entity.Navigation(t => t.KeyPoints)
                  .HasField("_keyPoints")
                  .UsePropertyAccessMode(PropertyAccessMode.Field);

            entity.HasMany(t => t.Equipment)
                  .WithMany()
                  .UsingEntity(j => { j.ToTable("TourEquipment"); });
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
            entity.Property(e => e.Status).IsRequired().HasConversion<string>();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => e.AuthorId);
            entity.HasIndex(e => e.Status);
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
    }
}