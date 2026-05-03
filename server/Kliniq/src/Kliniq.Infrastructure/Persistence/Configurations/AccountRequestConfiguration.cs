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
            
            builder.Property(a => a.Email)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(a => a.Specialization)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(a => a.PrcIdPath)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(a => a.BoardCertificatePath)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(a => a.MedicalDiplomaPath)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(a => a.CertificateOfGoodStandingPath)
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

            builder.Property(a => a.Status);

            builder.HasIndex(a => a.InvitationToken).IsUnique();
            builder.HasIndex(a => a.Email);
            builder.HasIndex(a => a.Status);

            builder.Property(a => a.CreatedAtUtc).IsRequired();
            builder.Property(a => a.UpdatedAtUtc);
            builder.Property(a => a.CreatedBy).HasMaxLength(100);
            builder.Property(a => a.UpdatedBy).HasMaxLength(100);

            builder.Ignore(a => a.DomainEvents);

        }
    }
}
