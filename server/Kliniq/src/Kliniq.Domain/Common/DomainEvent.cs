namespace Kliniq.Domain.Common
{
    public abstract class DomainEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
