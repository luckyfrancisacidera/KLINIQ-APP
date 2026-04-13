using Kliniq.Domain.Common;
using Kliniq.Domain.ValueObjects;

namespace Kliniq.Domain.Entities
{
    public class Clinic : AuditableEntity
    {
        public FullName Name { get; private set; } = null!;
        public GeoLocation Location { get; set; } = null!;

        private readonly List<Practioner> _practioners = new();

        public IReadOnlyCollection<Practioner> Practioners => _practioners.AsReadOnly();

        private Clinic() { }

        public Clinic(FullName name, GeoLocation location)
        {
            Id = Guid.NewGuid();
            Name = name;
            Location = location;
        }
    }
}
