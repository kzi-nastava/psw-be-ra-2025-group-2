using Explorer.Stakeholders.Core.Domain.Emergency;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Explorer.Stakeholders.Infrastructure.Database.Configurations.Emergency
{
    public class EmbassyConfiguration : IEntityTypeConfiguration<Embassy>
    {
        public void Configure(EntityTypeBuilder<Embassy> builder)
        {
            builder.ToTable("Embassies", "stakeholders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DirectoryId)
                .IsRequired();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Address)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(x => x.Phone)
                .HasMaxLength(50);

            builder.Property(x => x.Email)
                .HasMaxLength(150);

            builder.Property(x => x.Website)
                .HasMaxLength(200);
        }
    }
}
