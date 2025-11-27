using Explorer.Tours.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Explorer.Tours.Infrastructure.Database;

public class ToursContext : DbContext
{
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<TouristObject> TouristObject { get; set; }
    public DbSet<TouristEquipment> TouristEquipment { get; set; }
    public DbSet<TourProblem> TourProblems { get; set; }

    public DbSet<Tour> Tours { get; set; }
    public DbSet<Monument> Monument { get; set; }
    public ToursContext(DbContextOptions<ToursContext> options) : base(options) {}


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tours");
    }
}