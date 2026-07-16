namespace Kliniq.Application.Common.Interfaces
{
    public interface IAppTimeZone
    {
        DateTime LocalNow { get; }
        DateOnly Today { get; }
        DateTime ToUtc(DateOnly localDate, TimeOnly localTime);
        DateOnly ToLocalDate(DateTime utcDateTime);
        TimeOnly ToLocalTime(DateTime utcDateTime);
    }
}
