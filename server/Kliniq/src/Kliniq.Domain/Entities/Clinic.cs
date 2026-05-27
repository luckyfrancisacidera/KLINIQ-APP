using Kliniq.Domain.Common;
using Kliniq.Domain.ValueObjects;

namespace Kliniq.Domain.Entities
{
    public class Clinic : AuditableEntity
    {
        public string Name { get; private set; } = string.Empty;
        public GeoLocation Location { get; private set; } = default!;

        private readonly List<Practitioner> _practitioners = new();

        public IReadOnlyCollection<Practitioner> Practitioners => _practitioners.AsReadOnly();

        private Clinic() { }

        public Clinic(string name, GeoLocation location)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Clinic name is required", nameof(name));

            Id = Guid.NewGuid();
            Name = name;
            Location = location ?? throw new ArgumentNullException(nameof(location));
        }
    }
}
