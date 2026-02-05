using Explorer.Stakeholders.Core.Domain.Emergency;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Explorer.Stakeholders.Infrastructure.Database.Configurations.Emergency
{
    public class EmergencyPhraseConfiguration : IEntityTypeConfiguration<EmergencyPhrase>
    {
        public void Configure(EntityTypeBuilder<EmergencyPhrase> builder)
        {
            builder.ToTable("EmergencyPhrases", "stakeholders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DirectoryId)
                .IsRequired();

            builder.Property(x => x.Category)
                .IsRequired();

            builder.Property(x => x.MyText)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(x => x.LocalText)
                .IsRequired()
                .HasMaxLength(300);
        }
    }
}
