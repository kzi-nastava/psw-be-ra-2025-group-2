using Explorer.Payments.Core.Domain;
using Explorer.Stakeholders.Core.Domain.ShoppingCarts;
using Explorer.Stakeholders.Infrastructure.Database.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database;

public class PaymentsContext : DbContext
{
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<TourPurchaseToken> TourPurchaseTokens { get; set; }
    public DbSet<Coupon> Coupons { get; set; }

    public PaymentsContext(DbContextOptions<PaymentsContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Šema za Payments modul
        modelBuilder.HasDefaultSchema("payments");
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ShoppingCartConfiguration());

        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
    }
}
