using Kliniq.Domain.Common;

namespace Kliniq.Domain.ValueObjects
{
    public class Address : ValueObject
    {
        public string Street { get; } = null!;
        public string City { get; } = null!;
        public string Country { get; } = null!;

        private Address() { }

        public Address(string street, string city, string country)
        {
            Street = string.IsNullOrWhiteSpace(street)
                ? throw new ArgumentException("Street is required")
                : street;

            City = string.IsNullOrWhiteSpace(city)
                ? throw new ArgumentException("City is required")
                : city;

            Country = string.IsNullOrWhiteSpace(country)
                ? throw new ArgumentException("Country is required")
                : country;  
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return Country;
        }
    }
}
