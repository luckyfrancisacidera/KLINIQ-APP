namespace Kliniq.Domain.Common
{
    public abstract class ValueObject
    {
        protected abstract IEnumerable<(string Name, object Value)> GetEqualityComponents();

        public override bool Equals(object? obj)
        {
            if(obj is not ValueObject other || GetType() != obj.GetType())
                return false;

            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents().Aggregate(1, (current, obj) =>
            {
                unchecked
                {
                    return current * 23 + HashCode.Combine(obj.Name, obj.Value);
                }
            });
        }

        public static bool operator ==(ValueObject? left, ValueObject? right)
            => Equals(left, right);

        public static bool operator !=(ValueObject? left, ValueObject? right)
            => !Equals(left, right);

        public override string ToString()
        {
            return string.Join(", ",
                GetEqualityComponents().Select(x => $"{x.Name}: {x.Value}"));
        }
    }
}
