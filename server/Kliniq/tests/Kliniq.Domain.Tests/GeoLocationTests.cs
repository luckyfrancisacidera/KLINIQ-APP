using Kliniq.Domain.ValueObjects;

namespace Kliniq.Domain.Tests;

public sealed class GeoLocationTests
{
    [Theory]
    [InlineData(-90, -180)]
    [InlineData(90, 180)]
    [InlineData(18.1987, 120.5936)]
    public void ValidCoordinates_AreAccepted(double latitude, double longitude)
    {
        var location = new GeoLocation(latitude, longitude);
        Assert.Equal(latitude, location.Latitude);
        Assert.Equal(longitude, location.Longitude);
    }

    [Theory]
    [InlineData(-90.1, 0)]
    [InlineData(90.1, 0)]
    [InlineData(0, -180.1)]
    [InlineData(0, 180.1)]
    public void InvalidCoordinates_Throw(double latitude, double longitude)
        => Assert.Throws<ArgumentException>(() => new GeoLocation(latitude, longitude));
}
