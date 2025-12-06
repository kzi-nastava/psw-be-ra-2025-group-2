using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.ShoppingCarts;
using Explorer.Stakeholders.Infrastructure.Database.Configurations;
using Microsoft.EntityFrameworkCore;
namespace Explorer.Stakeholders.Infrastructure.Database;

public class StakeholdersContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<Meetup> Meetups { get; set; }
    public DbSet<TourPreferences> TourPreferences { get; set; }
    public DbSet<AuthorAwards> AuthorAwards { get; set; }
    public DbSet<AppRating> AppRatings { get; set; }
    public DbSet<Club> Clubs { get; set; }
    public DbSet<TouristPosition> TouristPositions { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    public StakeholdersContext(DbContextOptions<StakeholdersContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("stakeholders");

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

        modelBuilder.ApplyConfiguration(new ShoppingCartConfiguration());

        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());

        ConfigureStakeholder(modelBuilder);
    }

    private static void ConfigureStakeholder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<Person>(s => s.UserId);
    }
}