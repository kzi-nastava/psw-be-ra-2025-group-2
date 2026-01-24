using Explorer.Stakeholders.Core.Domain.Emergency;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Explorer.Stakeholders.Infrastructure.Database.Configurations
{
    public class EmergencyDirectoryConfiguration : IEntityTypeConfiguration<EmergencyDirectory>
    {
        public void Configure(EntityTypeBuilder<EmergencyDirectory> builder)
        {
            builder.ToTable("EmergencyDirectories", "stakeholders");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Instructions).IsRequired();
            builder.Property(x => x.Disclaimer).IsRequired();

            builder.OwnsOne(x => x.Country, cc =>
            {
                cc.Property(p => p.Value)
                  .HasColumnName("CountryCode")
                  .HasMaxLength(3)
                  .IsRequired();

                cc.HasIndex(p => p.Value).IsUnique();
            });

            
            builder.HasMany(x => x.Places)
                   .WithOne()
                   .HasForeignKey("DirectoryId")
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(x => x.Places)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Metadata
                   .FindNavigation(nameof(EmergencyDirectory.Places))!
                   .SetField("_places");
        }
    }
}
