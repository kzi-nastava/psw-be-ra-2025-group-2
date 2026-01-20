using Explorer.Stakeholders.Core.Domain.Planner;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Infrastructure.Database.Configurations
{
    public class DayEntryConfiguration : IEntityTypeConfiguration<DayEntry>
    {
        public void Configure(EntityTypeBuilder<DayEntry> builder)
        {
            builder.ToTable("PlannerDayEntries");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Date)
                   .IsRequired();

            builder.OwnsMany(x => x.Entries, navigationBuilder =>
            {
                navigationBuilder.ToTable("PlannerScheduleEntries");

                navigationBuilder.HasKey(x => x.Id);

                navigationBuilder.Property<long>("DayEntryId");

                navigationBuilder.OwnsOne(x => x.ScheduledTime, timeBuilder =>
                {
                    timeBuilder.Property(t => t.Start)
                               .HasColumnName("StartTime")
                               .IsRequired();

                    timeBuilder.Property(t => t.End)
                               .HasColumnName("EndTime")
                               .IsRequired();
                });

                navigationBuilder.Property(x => x.TourId)
                                 .IsRequired();
            });

            var navigation = builder.Metadata.FindNavigation(nameof(DayEntry.Entries));
            navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
