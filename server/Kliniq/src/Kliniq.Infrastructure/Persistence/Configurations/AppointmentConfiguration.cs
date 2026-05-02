using Kliniq.Domain.Common;
using Kliniq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kliniq.Infrastructure.Persistence.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id).ValueGeneratedNever();

            builder.Property(a => a.PatientId).IsRequired();
            builder.Property(a => a.DoctorId).IsRequired();
            builder.Property(a => a.ClinicId).IsRequired();

            builder.Property(a => a.ScheduledAt).IsRequired();

            builder.Property(a => a.Duration)
                .HasConversion(
                    ts => (long)ts.TotalMinutes,
                    minutes => TimeSpan.FromMinutes(minutes))
                .HasColumnName("DurationMinutes")
                .IsRequired();
            
            builder.Property(a => a.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(a => a.Reason)
                .HasMaxLength(500);


            builder.HasIndex(a => a.PatientId);
            builder.HasIndex(a => a.DoctorId);
            builder.HasIndex(a => a.ClinicId);
            builder.HasIndex(a => a.ScheduledAt);

            builder.Property(a => a.CreatedAtUtc).IsRequired();
            builder.Property(a => a.UpdatedAtUtc);
            builder.Property(a => a.CreatedBy).HasMaxLength(100);
            builder.Property(a => a.UpdatedBy).HasMaxLength(100);

            builder.Ignore(a => a.DomainEvents);

        } 
    }
}
