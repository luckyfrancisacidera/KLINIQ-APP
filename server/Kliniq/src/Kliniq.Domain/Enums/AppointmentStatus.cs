namespace Kliniq.Domain.Enums
{
    public enum AppointmentStatus
    {
        Pending = 0,
        Confirmed = 1,
        InQueue = 2,
        InProgress = 3,
        Completed = 4,
        Cancelled = 5,
        NoShow = 6,
        Rescheduled = 7
    }
}
