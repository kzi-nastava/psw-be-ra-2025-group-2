using Explorer.Stakeholders.Core.Domain.ShoppingCarts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Explorer.Stakeholders.Infrastructure.Database.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ShoppingCartId)
            .IsRequired();

        builder.Property(x => x.TourId)
            .IsRequired(false);

        builder.Property(x => x.BundleId)
           .IsRequired(false);

        builder.Property(x => x.ItemType)
         .IsRequired()
         .HasMaxLength(20);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);


        builder.Property(x => x.AuthorId)
            .IsRequired();

        builder.Property(x => x.TourIds)
        .HasConversion(
            v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
            v => System.Text.Json.JsonSerializer.Deserialize<List<long>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new List<long>()
        )
        .HasColumnName("TourIds")
        .HasColumnType("text")
        .IsRequired();


        builder.OwnsOne(x => x.Price, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Price")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("PriceCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        
    }
}