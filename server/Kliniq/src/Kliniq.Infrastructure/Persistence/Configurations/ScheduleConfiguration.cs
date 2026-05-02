using Kliniq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kliniq.Infrastructure.Persistence.Configurations
{
    public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            builder.ToTable("SChedules");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id).ValueGeneratedNever();

            builder.Property(s => s.PractitionerId).IsRequired();

            builder.Property(s => s.Day)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(s => s.StartTime)
                .HasColumnType("time")
                .IsRequired();

            builder.Property(s => s.EndTime)
                .HasColumnType("time")
                .IsRequired();

            builder.Property(s => s.IsAvailable)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasOne(s => s.Practioner)
                .WithMany()
                .HasForeignKey(s => s.PractitionerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(s => new {s.PractitionerId, s.StartTime })
                .IsUnique();

            builder.Property(s => s.CreatedAtUtc).IsRequired();
            builder.Property(s => s.UpdatedAtUtc);
            builder.Property(s => s.CreatedBy).HasMaxLength(100);
            builder.Property(s => s.UpdatedBy).HasMaxLength(100);

            builder.Ignore(s => s.DomainEvents);
        }
    }
}
