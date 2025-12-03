using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.Execution;
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

    public DbSet<TourExecution> TourExecutions { get; set; }

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
                kp.Property(k => k.Latitude);
                kp.Property(k => k.Longitude);
            });
        });
    }
}