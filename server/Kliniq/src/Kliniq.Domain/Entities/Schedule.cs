using Kliniq.Domain.Common;
using Kliniq.Domain.Enums;

namespace Kliniq.Domain.Entities
{
    public class Schedule : AuditableEntity
    {
        public Guid PractitionerId { get; private set; }
        public Practitioner? Practioner { get; private set; }
        public ClinicDayOfWeek Day { get; private set; }
        public TimeOnly StartTime { get; private set; }
        public TimeOnly EndTime { get; private set; }

        public int AppointmentLengthMinutes { get; private set; }

        public bool IsAvailable { get; private set; }

        private readonly List<ScheduleBreak> _breaks = new();
        public IReadOnlyCollection<ScheduleBreak> Breaks => _breaks.AsReadOnly();

        private Schedule(){ }

        public Schedule(Guid practionerId, ClinicDayOfWeek day, TimeOnly startTime, TimeOnly endTime, int appointmentLengthMinutes)
        {
            if(endTime <= startTime)
                throw new DomainException("End time must be after start time");

            if((endTime - startTime).TotalMinutes < appointmentLengthMinutes)
                throw new DomainException("Schedule must be at least one appointment length long");

            if(appointmentLengthMinutes < 10)
                throw new DomainException("Appointment length must be at least 10 minutes");

            Id = Guid.NewGuid();
            PractitionerId = practionerId;
            Day = day;
            StartTime = startTime;
            EndTime = endTime;
            AppointmentLengthMinutes = appointmentLengthMinutes;
            IsAvailable = true;
        }

        public void AddBreak(TimeOnly breakStart, TimeOnly breakEnd)
        {
            if(breakStart >= breakEnd)
                throw new DomainException("Break end must be after break start");

            if(breakStart < StartTime || breakEnd > EndTime)
                throw new DomainException("Break must be within schedule time");

            bool overlaps = _breaks.Any(b =>breakStart < b.EndTime && breakEnd > b.StartTime);

            if(overlaps)
                throw new DomainException("Break overlaps with existing break");

            _breaks.Add(new ScheduleBreak(Id, breakStart, breakEnd));
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void RemoveBreak(Guid breakId)
        {
            var brk = _breaks.FirstOrDefault(b => b.Id == breakId) ?? throw new DomainException("Break not found on this schedule");
            _breaks.Remove(brk);
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void UpdateTimeSlot(TimeOnly newStart, TimeOnly newEnd, int newLengthMinutes)
        {
            if(newEnd <= newStart)
                throw new DomainException("End time must be after start time");

            if ((newEnd - newStart).TotalMinutes < newLengthMinutes)
                throw new DomainException("Schedule must be at least one appointment length long");

            StartTime = newStart;
            EndTime= newEnd;
            AppointmentLengthMinutes = newLengthMinutes;
            _breaks.Clear();
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void MarkUnavailable()
        {
            if(!IsAvailable)
                throw new DomainException("Schedule is already unavailable");
            IsAvailable = false;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void MarkAvailable()
        {
            if (IsAvailable)
                throw new DomainException("Schedule is already available");
            IsAvailable = true;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public IReadOnlyList<TimeOnly> GetAvailableSlots()
        {
            var slots = new List<TimeOnly>();
            var cursor = StartTime;

            while (cursor.AddMinutes(AppointmentLengthMinutes) <= EndTime)
            {
                var slotEnd = cursor.AddMinutes(AppointmentLengthMinutes);

                bool inBreak = _breaks.Any(b => cursor < b.EndTime && slotEnd > b.StartTime);
                if (!inBreak)
                    slots.Add(cursor);

                cursor = slotEnd;
            }

            return slots.AsReadOnly();
        }

        public bool CoversTimeSlot(TimeOnly requestedStart, TimeOnly requestedEnd)
        {
            if(!IsAvailable) return false;
            if (requestedStart < StartTime || requestedEnd > EndTime) return false;

            bool inBreak = _breaks.Any(b => requestedStart < b.EndTime && requestedEnd > b.StartTime);
            return !inBreak;
        }
    }
}
