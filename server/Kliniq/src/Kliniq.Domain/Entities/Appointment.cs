using Kliniq.Domain.Common;
using Kliniq.Domain.Enums;

namespace Kliniq.Domain.Entities
{
    public class Appointment : AuditableEntity
    {
        public Guid PatientId { get; private set; }
        public Guid PractitionerId { get; private set; }
        public Guid ClinicId { get; private set; }

        public  DateTime ScheduledAt { get; private set; }
        public TimeSpan Duration { get; private set; }
        public AppointmentStatus Status { get; private set; }
        public DateTime EndTime { get; private set; }
        public string? Reason { get; private set; }
        public string? Notes { get; private set; }

        private Appointment() { }

        public Appointment(
            Guid patientId,
            Guid practitionerId,
            Guid clinicId,
            DateTime scheduledAt, 
            TimeSpan duration,
            string? reason)
        {
            if (scheduledAt < DateTime.UtcNow)
                throw new ArgumentException("Scheduled time cannot be in the past");

            if (duration < TimeSpan.Zero)
                throw new ArgumentException("Duration must be a positive time span");

            Id = Guid.NewGuid();
            PatientId = patientId;
            PractitionerId = practitionerId;
            ClinicId = clinicId;
            ScheduledAt = scheduledAt;
            Duration = duration;
            Status = AppointmentStatus.Pending;
            EndTime = scheduledAt.Add(duration);
            Reason = reason;
        }

        public void Confirm()
        {
            if(Status != AppointmentStatus.Pending)
                throw new DomainException("Only pending appointments can be confirmed");

            Status = AppointmentStatus.Confirmed;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == AppointmentStatus.Completed)
                throw new DomainException("Cannot cancel completed appointment");

            if (Status == AppointmentStatus.Cancelled)
                throw new DomainException("Appointment is already cancelled");

            Status = AppointmentStatus.Cancelled;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void Complete(string? notes = null)
        {
            if (Status != AppointmentStatus.Confirmed)
                throw new DomainException("Only confirmed appointments can be completed");

            Status = AppointmentStatus.Completed;
            Notes = notes;
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }
}
