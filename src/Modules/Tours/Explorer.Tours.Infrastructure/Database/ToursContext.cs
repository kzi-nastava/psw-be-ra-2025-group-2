using Explorer.Tours.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Explorer.ShoppingCart.Core.Domain;
namespace Explorer.Tours.Infrastructure.Database;

public class ToursContext : DbContext
{
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<TouristObject> TouristObject { get; set; }
    public DbSet<TouristEquipment> TouristEquipment { get; set; }
    public DbSet<TourProblem> TourProblems { get; set; }

    public DbSet<Tour> Tours { get; set; }
    public DbSet<Monument> Monument { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<TourPurchaseToken> TourPurchaseTokens { get; set; }
    public DbSet<Explorer.ShoppingCart.Core.Domain.Cart> ShoppingCarts { get; set; } 
    public DbSet<OrderItem> OrderItems { get; set; }
    public ToursContext(DbContextOptions<ToursContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tours");
        modelBuilder.Entity<Explorer.ShoppingCart.Core.Domain.Cart>().ToTable("ShoppingCarts", "shoppingcart");
        modelBuilder.Entity<OrderItem>().ToTable("OrderItems", "shoppingcart");

        modelBuilder.Entity<Explorer.ShoppingCart.Core.Domain.Cart>()
        .HasMany(sc => sc.Items)
        .WithOne()
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Tour>()
        .Property(t => t.Tags)
        .HasColumnType("jsonb");
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

            builder
                .HasMany(t => t.Equipment)
                .WithMany()
                .UsingEntity(j =>
                {
                    j.ToTable("TourEquipment");
                });
        });
    }
}