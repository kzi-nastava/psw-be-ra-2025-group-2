using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.Core.Domain.Wallets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Explorer.Payments.Infrastructure.Database.Configurations
{
    public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Amount).IsRequired();
            builder.Property(t => t.Type).IsRequired();
            builder.Property(t => t.CreatedAt).IsRequired();

            builder.ToTable("WalletTransactions");
        }
    }
}
