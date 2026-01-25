using Explorer.Stakeholders.Core.Domain.Emergency;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Explorer.Stakeholders.Infrastructure.Database.Configurations.Emergency
{
    public class EmergencyPlaceConfiguration : IEntityTypeConfiguration<EmergencyPlace>
    {
        public void Configure(EntityTypeBuilder<EmergencyPlace> builder)
        {
            builder.ToTable("EmergencyPlaces", "stakeholders");


            builder.HasKey(x => x.Id);

            builder.Property(x => x.DirectoryId)
                .IsRequired();

            builder.Property(x => x.Type)
                .IsRequired();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Address)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(x => x.Phone)
                .HasMaxLength(50);

            
           
        }
    }
}
