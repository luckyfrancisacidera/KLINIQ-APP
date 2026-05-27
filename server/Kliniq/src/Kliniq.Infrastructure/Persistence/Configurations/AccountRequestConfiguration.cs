using Kliniq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kliniq.Infrastructure.Persistence.Configurations
{
    public class AccountRequestConfiguration : IEntityTypeConfiguration<AccountRequest>
    {
        public void Configure(EntityTypeBuilder<AccountRequest> builder)
        {
            builder.ToTable("AccountRequests");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id).ValueGeneratedNever();

            builder.OwnsOne(a => a.Name, name =>
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

            builder.OwnsOne(a => a.Address, address =>
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
            
            builder.Property(a => a.ClinicName)
                .HasColumnName("ClinicName")
                .HasMaxLength(200)
                .IsRequired();

            builder.OwnsOne(a => a.ClinicLocation, geo =>
            {
                geo.Property(g => g.Latitude)
                    .HasColumnName("ClinicLatitude")
                    .HasColumnType("decimal(9,6)")
                    .IsRequired();

                geo.Property(g => g.Longitude)
                    .HasColumnName("ClinicLongitude")
                    .HasColumnType("decimal(9,6)")
                    .IsRequired();
            });

            builder.Property(a => a.Email)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(a => a.LicenseNumber)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property<string>("_specializations")
                .HasColumnName("Specializations")
                .HasMaxLength(150)
                .IsRequired()
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Property(a => a.PrcLicensePath)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(a => a.GovernmentIdPath)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(a => a.ProfessionalPhotoPath)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(a => a.CvPath)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(a => a.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(a => a.AdminNote)
                .HasMaxLength(500);

            builder.Property(a => a.InvitationToken)
                .HasMaxLength(100);

            builder.Property(a => a.InvitationExpiresAt);
            builder.Property(a => a.IsInvitationUsed);

            builder.HasIndex(a => a.InvitationToken).IsUnique();
            builder.HasIndex(a => a.Email);
            builder.HasIndex(a => a.Status);

            builder.Property(a => a.CreatedAtUtc).IsRequired();
            builder.Property(a => a.UpdatedAtUtc);
            builder.Property(a => a.CreatedBy).HasMaxLength(100);
            builder.Property(a => a.UpdatedBy).HasMaxLength(100);

            builder.Ignore(a => a.DomainEvents);
            builder.Ignore(a => a.Specializations);
            builder.Ignore(a => a.SpecializationsRaw);

        }
    }
}
