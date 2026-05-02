using Kliniq.Domain.Common;
using Kliniq.Domain.Enums;

namespace Kliniq.Domain.Entities
{
    public class Schedule : AuditableEntity
    {
        public Guid PractitionerId { get; private set; }
        public Practioner? Practioner { get; private set; }
        public ClinicDayOfWeek Day { get; private set; }
        public TimeOnly StartTime { get; private set; }
        public TimeOnly EndTime { get; private set; }
        public bool IsAvailable { get; set; }

        private Schedule(){ }

        public Schedule(Guid practionerId, ClinicDayOfWeek day, TimeOnly startTime, TimeOnly endTime)
        {
            if(endTime <= startTime)
                throw new ArgumentException("End time must be after start time");

            if((endTime - startTime).TotalMinutes < 30)
                throw new ArgumentException("Schedule must be at least 30 minutes long");

            Id = Guid.NewGuid();
            PractitionerId = practionerId;
            Day = day;
            StartTime = startTime;
            EndTime = endTime;
            IsAvailable = true;
        }

        public void MarkUnavailable()
        {
            if(!IsAvailable)
                throw new DomainException("Schedule is already unavailable");

            IsAvailable = false;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void UpdateTimeSlot(TimeOnly newStart, TimeOnly newEnd)
        {
            if(newEnd <= newStart)
                throw new ArgumentException("End time must be after start time");

            if ((newEnd - newStart).TotalMinutes < 30)
                throw new ArgumentException("Schedule must be at least 30 minutes long");

            StartTime = newStart;
            EndTime= newEnd;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public bool CoversTimeSlot(TimeOnly requestedStart, TimeOnly requestedEnd)
            => IsAvailable && requestedStart >= StartTime && requestedEnd <= EndTime;


    }
}
