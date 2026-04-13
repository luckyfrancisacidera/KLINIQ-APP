using Kliniq.Domain.Common;

namespace Kliniq.Domain.ValueObjects
{
    public class GeoLocation : ValueObject
    {
        public double Latitude { get; }
        public double Longitude { get;}

        private GeoLocation() { }

        public GeoLocation(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("Invalid latitude value. Must be between -90 and 90.");

            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("Invalid longitude value. Must be between -180 and 180.");
            Latitude = latitude;
            Longitude = longitude;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Latitude;
            yield return Longitude;
        }

    }
}
