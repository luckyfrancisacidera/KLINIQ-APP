using Kliniq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kliniq.Infrastructure.Persistence.Configurations
{
    public class ClinicOperatingHoursConfiguration : IEntityTypeConfiguration<ClinicOperatingHours>
    {
        public void Configure(EntityTypeBuilder<ClinicOperatingHours> builder)
        {
            builder.ToTable("ClinicOperatingHours");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedNever();

            builder.Property(c => c.ClinicId).IsRequired();

            builder.Property(c => c.Day)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(c => c.OpenTime)
                .HasColumnType("time")
                .IsRequired();

            builder.Property(c => c.CloseTime)
                .HasColumnType("time")
                .IsRequired();

            builder.Property(c => c.IsClosed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(c => c.Clinic)
                .WithMany()
                .HasForeignKey(c => c.ClinicId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(c => new { c.ClinicId, c.Day }).IsUnique();

            builder.Property(c => c.CreatedAtUtc).IsRequired();
            builder.Property(c => c.UpdatedAtUtc);
            builder.Property(c => c.CreatedBy).HasMaxLength(100);
            builder.Property(c => c.UpdatedBy).HasMaxLength(100);

            builder.Ignore(c => c.DomainEvents);
        }
    }
}
