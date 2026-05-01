using Kliniq.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Kliniq.Infrastructure.Identity
{
    public class AppUser : IdentityUser
    {
        public UserRole Role { get; set; }
        public Guid? PatientId { get; set; }
        public Guid? PractitionerId { get; set; }
    }
}
