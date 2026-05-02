using Kliniq.Domain.Entities;
using Kliniq.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Kliniq.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Practitioner> Practioners => Set<Practitioner>();
        public DbSet<Clinic> Clinics => Set<Clinic>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<AccountRequest> AccountRequests => Set<AccountRequest>();
        public DbSet<ClinicOperatingHours> ClinicOperatingHours => Set<ClinicOperatingHours>();
        public DbSet<Schedule> Schedules => Set<Schedule>();
  
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
