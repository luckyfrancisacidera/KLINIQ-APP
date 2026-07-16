using Kliniq.Infrastructure.Services;
using Microsoft.Extensions.Configuration;

namespace Kliniq.Tests.Infrastructure;

public sealed class AppTimeZoneTests
{
    private static AppTimeZone CreateService() => new(
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["App:TimeZoneId"] = "Asia/Manila"
            })
            .Build());

    [Fact]
    public void ToUtc_InterpretsScheduleTimeInPhilippineTime()
    {
        var service = CreateService();

        var utc = service.ToUtc(new DateOnly(2026, 7, 11), new TimeOnly(9, 0));

        Assert.Equal(new DateTime(2026, 7, 11, 1, 0, 0, DateTimeKind.Utc), utc);
    }

    [Fact]
    public void UtcRoundTrip_ReturnsClinicLocalDateAndTime()
    {
        var service = CreateService();
        var utc = new DateTime(2026, 7, 11, 1, 30, 0, DateTimeKind.Utc);

        Assert.Equal(new DateOnly(2026, 7, 11), service.ToLocalDate(utc));
        Assert.Equal(new TimeOnly(9, 30), service.ToLocalTime(utc));
    }
}
