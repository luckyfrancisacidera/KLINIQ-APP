using Kliniq.Domain.Common;

namespace Kliniq.Domain.Entities
{
    public class Patient : AuditableEntity
    {
        public Guid UserId { get; private set; }
        
        public string FirstName { get; private set; } = string.Empty;   


        private Patient() { }
    }
}
