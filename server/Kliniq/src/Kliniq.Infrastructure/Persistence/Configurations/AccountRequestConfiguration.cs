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

            builder.Property(a => a.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(a => a.LastName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(a => a.Email)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(a => a.LicenseNumber)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(a => a.Specialization)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(a => a.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(a => a.AdminNote)
                .HasMaxLength(500);

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
