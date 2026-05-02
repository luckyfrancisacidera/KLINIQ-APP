using Kliniq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kliniq.Infrastructure.Persistence.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("Patients");

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


            builder.OwnsOne(p => p.Address, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("Street")
                    .HasMaxLength(200)
                    .IsRequired();

                address.Property(a => a.City)
                    .HasColumnName("City")
                    .HasMaxLength(100)
                    .IsRequired();

                address.Property(a => a.Country)
                    .HasColumnName("Country")
                    .HasMaxLength(100)
                    .IsRequired();
            });


            builder.Property(p => p.DateOfBirth)
                .IsRequired();

            builder.Property(p => p.Gender)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(p => p.PhoneNumber)
                .HasMaxLength(20);
            
            builder.Property(p => p.EmergencyContact)
                .HasMaxLength(20);

            builder.HasIndex(p => p.UserId).IsUnique();

            builder.Property(p => p.CreatedAtUtc).IsRequired();
            builder.Property(p => p.UpdatedAtUtc);
            builder.Property(p => p.CreatedBy).HasMaxLength(100);
            builder.Property(p => p.UpdatedBy).HasMaxLength(100);

            builder.Ignore(p => p.Age);
            builder.Ignore(p => p.Appointments);
            builder.Ignore(p => p.DomainEvents);



        }
    }
}
