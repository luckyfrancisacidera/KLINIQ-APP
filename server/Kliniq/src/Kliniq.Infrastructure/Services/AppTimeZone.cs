using Kliniq.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Kliniq.Infrastructure.Services
{
    public sealed class AppTimeZone : IAppTimeZone
    {
        private readonly TimeZoneInfo _timeZone;

        public AppTimeZone(IConfiguration configuration)
        {
            var configuredId = configuration["App:TimeZoneId"];
            if (string.IsNullOrWhiteSpace(configuredId))
                throw new InvalidOperationException("App:TimeZoneId is not configured.");

            _timeZone = Resolve(configuredId);
        }

        public DateTime LocalNow => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timeZone);
        public DateOnly Today => DateOnly.FromDateTime(LocalNow);

        public DateTime ToUtc(DateOnly localDate, TimeOnly localTime)
        {
            var unspecified = DateTime.SpecifyKind(localDate.ToDateTime(localTime), DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(unspecified, _timeZone);
        }

        public DateOnly ToLocalDate(DateTime utcDateTime)
            => DateOnly.FromDateTime(ToLocalDateTime(utcDateTime));

        public TimeOnly ToLocalTime(DateTime utcDateTime)
            => TimeOnly.FromDateTime(ToLocalDateTime(utcDateTime));

        private DateTime ToLocalDateTime(DateTime utcDateTime)
        {
            var normalizedUtc = utcDateTime.Kind == DateTimeKind.Utc
                ? utcDateTime
                : DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            return TimeZoneInfo.ConvertTimeFromUtc(normalizedUtc, _timeZone);
        }

        private static TimeZoneInfo Resolve(string configuredId)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(configuredId);
            }
            catch (TimeZoneNotFoundException) when (configuredId.Equals("Asia/Manila", StringComparison.OrdinalIgnoreCase))
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
            }
            catch (InvalidTimeZoneException exception)
            {
                throw new InvalidOperationException($"Configured time zone '{configuredId}' is invalid.", exception);
            }
        }
    }
}
