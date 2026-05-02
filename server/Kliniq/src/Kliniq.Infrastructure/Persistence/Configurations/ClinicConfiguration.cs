using Kliniq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kliniq.Infrastructure.Persistence.Configurations
{
    public class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
    {
        public void Configure(EntityTypeBuilder<Clinic> builder)
        {
            builder.ToTable("Clinics");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id).ValueGeneratedNever();

            builder.Property(c => c.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.OwnsOne(c => c.Location, geo =>
            {
                geo.Property(g => g.Latitude)
                    .HasColumnName("Lattitude")
                    .HasColumnType("decimal(9,6)")
                    .IsRequired();

                geo.Property(g => g.Longitude)
                    .HasColumnName("Longitude")
                    .HasColumnType("decimal(9,6)")
                    .IsRequired();
            });

            builder.Ignore(c => c.DomainEvents);

            builder.Property(c => c.CreatedAtUtc).IsRequired();
            builder.Property(c => c.UpdatedAtUtc);
            builder.Property(c => c.CreatedBy).HasMaxLength(100);
            builder.Property(c => c.UpdatedBy).HasMaxLength(100);

        }
    }
}
