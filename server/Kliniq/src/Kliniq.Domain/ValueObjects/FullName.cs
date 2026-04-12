using Kliniq.Domain.Common;

namespace Kliniq.Domain.ValueObjects
{
    public class FullName : ValueObject
    {
        public string FirstName { get; } = null!;
        public string LastName { get; } = null!;

        private FullName() { }

        public FullName(string firstName, string lastName)
        {
            FirstName = string.IsNullOrWhiteSpace(firstName) 
                ? throw new ArgumentException("First name is required") 
                : firstName;

            LastName = string.IsNullOrWhiteSpace(lastName)
                ? throw new ArgumentException("Last name is required")
                : lastName;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
            
        }
            }
}
