using Explorer.Payments.Core.Domain.ShoppingCarts;
using Explorer.Payments.Core.Domain.Wallets;
using Explorer.Payments.Infrastructure.Database.Configurations;
using Explorer.Stakeholders.Core.Domain.ShoppingCarts;
using Explorer.Stakeholders.Infrastructure.Database.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database;

public class PaymentsContext : DbContext
{
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<TourPurchaseToken> TourPurchaseTokens { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<WalletTransaction> WalletTransactions { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<PaymentRecord> PaymentRecords { get; set; }

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

        modelBuilder.ApplyConfiguration(new WalletConfiguration());
        modelBuilder.ApplyConfiguration(new WalletTransactionConfiguration());

    }
}
