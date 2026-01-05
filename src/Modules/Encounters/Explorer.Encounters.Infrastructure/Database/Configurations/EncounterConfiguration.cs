using Explorer.Encounters.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Explorer.Encounters.Infrastructure.Database.Configurations
{
    internal class EncounterConfiguration : IEntityTypeConfiguration<Encounter>
    {
        public void Configure(EntityTypeBuilder<Encounter> builder)
        {
            builder.Property(e => e.Name).IsRequired();
            builder.Property(e => e.Description).IsRequired();

            builder.OwnsOne(e => e.XP, xp =>
            {
                xp.Property(p => p.Value).HasColumnName("XP").IsRequired();
            });

            builder.OwnsOne(e => e.Location, l =>
            {
                l.Property(p => p.Latitude).HasColumnName("Latitude").IsRequired();
                l.Property(p => p.Longitude).HasColumnName("Longitude").IsRequired();
            });

            builder.HasDiscriminator<EncounterType>("Type")
                .HasValue<SocialEncounter>(EncounterType.Social)
                .HasValue<HiddenLocationEncounter>(EncounterType.Location)
                .HasValue<MiscEncounter>(EncounterType.Miscellaneous);

        }
    }
}