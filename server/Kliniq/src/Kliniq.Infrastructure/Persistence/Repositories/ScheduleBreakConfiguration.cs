using Kliniq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kliniq.Infrastructure.Persistence.Configurations
{
    public class ScheduleBreakConfiguration : IEntityTypeConfiguration<ScheduleBreak>
    {
        public void Configure(EntityTypeBuilder<ScheduleBreak> builder)
        {
            builder.ToTable("ScheduleBreaks");

            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id).ValueGeneratedNever();

            builder.Property(b => b.ScheduleId).IsRequired();

            builder.Property(b => b.StartTime)
                .HasColumnType("time")
                .IsRequired();

            builder.Property(b => b.EndTime)
                .HasColumnType("time")
                .IsRequired();

            builder.Ignore(b => b.DomainEvents);
        }
    }
}