using Kliniq.Domain.Entities;
using Kliniq.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kliniq.Infrastructure.Persistence
{
    public class AppDbcontext : IdentityDbContext<AppUser>
    {
        public AppDbcontext(DbContextOptions<AppDbcontext> options) : base(options) { }

        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Practioner> Practioners => Set<Practioner>();
        public DbSet<Clinic> Clinics => Set<Clinic>();
        public DbSet<Appointment> Appointments => Set<Appointment>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbcontext).Assembly);
        }
    }
}
