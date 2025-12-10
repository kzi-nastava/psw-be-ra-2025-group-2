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
    public ToursContext(DbContextOptions<ToursContext> options) : base(options) {}


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
    }
}