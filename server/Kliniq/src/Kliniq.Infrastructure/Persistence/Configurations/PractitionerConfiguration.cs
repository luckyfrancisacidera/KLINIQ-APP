using Kliniq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kliniq.Infrastructure.Persistence.Configurations
{
    public class PractitionerConfiguration : IEntityTypeConfiguration<Practitioner>
    {
        public void Configure(EntityTypeBuilder<Practitioner> builder)
        {
            builder.ToTable("Practitioners");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();

            builder.Property(p => p.UserId).IsRequired();

            builder.OwnsOne(p => p.Name, name =>
            {
                name.Property(n => n.FirstName)
                    .HasColumnName("FirstName")
                    .HasMaxLength(50)
                    .IsRequired();

                name.Property(n => n.LastName)
                    .HasColumnName("LastName")
                    .HasMaxLength(50)
                    .IsRequired();
            });

            builder.Property(p => p.LicenseNumber)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.Specialization)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.ClinicID)
                .IsRequired();

            builder.HasOne(p => p.Clinic)
                .WithMany(c => c.Practioners)
                .HasForeignKey(p => p.ClinicID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.UserId).IsUnique();
            builder.HasIndex(p => p.ClinicID);

            builder.Property(p => p.CreatedAtUtc).IsRequired();
            builder.Property(p => p.UpdatedAtUtc);
            builder.Property(p => p.CreatedBy).HasMaxLength(100);
            builder.Property(p => p.UpdatedBy).HasMaxLength(100);


            builder.Ignore(p => p.Schedules);
            builder.Ignore(p => p.Appointments);
            builder.Ignore(p => p.DomainEvents);


        }
    }
}
