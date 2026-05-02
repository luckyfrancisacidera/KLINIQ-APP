using Kliniq.Domain.Common;
using Kliniq.Domain.Enums;

namespace Kliniq.Domain.Entities
{
    public class ClinicOperatingHours : AuditableEntity
    {
        public Guid ClinicId { get; private set; }
        public Clinic? Clinic { get; private set; }

        public ClinicDayOfWeek Day { get; private set; }
        public TimeOnly OpenTime { get; private set; }
        public TimeOnly CloseTime { get; private set; }

        public bool IsClosed { get; private set; }
        
        private ClinicOperatingHours() { }

        public ClinicOperatingHours(Clinic clinic, ClinicDayOfWeek day, TimeOnly openTime, TimeOnly closeTime, bool isClosed)
        {
            if (closeTime <= openTime)
                throw new ArgumentException("Close time must be after open time");
            
            Id = Guid.NewGuid();
            Clinic = clinic;
            ClinicId = clinic.Id;
            Day = day;
            OpenTime = openTime;
            CloseTime = closeTime;
            IsClosed = false;
        }


        public void MarkClosed()
        {
            IsClosed = true;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void MarkOpen()
        {
            IsClosed = false;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void UpdateOperatingHours(TimeOnly newOpenTime, TimeOnly newCloseTime)
        {
            if (newCloseTime <= newOpenTime)
                throw new ArgumentException("Close time must be after open time");
            OpenTime = newOpenTime;
            CloseTime = newCloseTime;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public bool isWithinOperatingHours(TimeOnly requestedStart, TimeOnly requestedEnd)
            => !IsClosed && requestedStart >= OpenTime && requestedEnd <= CloseTime;
    }
}
