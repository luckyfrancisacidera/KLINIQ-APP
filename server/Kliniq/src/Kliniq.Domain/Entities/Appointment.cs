using Kliniq.Domain.Common;
using Kliniq.Domain.Enums;

namespace Kliniq.Domain.Entities
{
    public class Appointment : AuditableEntity
    {
        public Guid PatientId { get; private set; }
        public Guid PractitionerId { get; private set; }
        public Guid ClinicId { get; private set; }

        public DateTime ScheduledAt { get; private set; }
        public TimeSpan Duration { get; private set; }
        public AppointmentStatus Status { get; private set; }
        public DateTime EndTime { get; private set; }
        public string? Reason { get; private set; }
        public string? Notes { get; private set; }
        public DateTime? QueuedAtUtc { get; private set; }
        public DateTime? ConsultationStartedAtUtc { get; private set; }
        public DateTime? CompletedAtUtc { get; private set; }

        private Appointment() { }

        public Appointment(
            Guid patientId,
            Guid practitionerId,
            Guid clinicId,
            DateTime scheduledAt,
            TimeSpan duration,
            string? reason)
        {
            if (scheduledAt <= DateTime.UtcNow)
                throw new ArgumentException("Scheduled time cannot be in the past");

            if (duration <= TimeSpan.Zero)
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

        public void Reschedule(DateTime scheduledAt, TimeSpan duration)
        {
            if (Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled or AppointmentStatus.InQueue or AppointmentStatus.InConsultation)
                throw new DomainException("Queued, active, completed, or cancelled appointments cannot be rescheduled");

            if (scheduledAt <= DateTime.UtcNow)
                throw new DomainException("Scheduled time must be in the future");

            if (duration <= TimeSpan.Zero)
                throw new DomainException("Duration must be positive");

            ScheduledAt = scheduledAt;
            Duration = duration;
            EndTime = scheduledAt.Add(duration);
            Status = AppointmentStatus.Pending;
            QueuedAtUtc = null;
            ConsultationStartedAtUtc = null;
            CompletedAtUtc = null;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void Confirm()
        {
            if (Status != AppointmentStatus.Pending)
                throw new DomainException("Only pending appointments can be confirmed");

            Status = AppointmentStatus.Confirmed;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void JoinQueue(DateTime queuedAtUtc)
        {
            if (Status != AppointmentStatus.Confirmed)
                throw new DomainException("Only confirmed appointments can enter the clinic queue");

            Status = AppointmentStatus.InQueue;
            QueuedAtUtc = NormalizeUtc(queuedAtUtc);
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void StartConsultation(DateTime startedAtUtc)
        {
            if (Status != AppointmentStatus.InQueue)
                throw new DomainException("Only queued appointments can start a consultation");

            Status = AppointmentStatus.InConsultation;
            ConsultationStartedAtUtc = NormalizeUtc(startedAtUtc);
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == AppointmentStatus.Completed)
                throw new DomainException("Cannot cancel a completed appointment");

            if (Status == AppointmentStatus.InConsultation)
                throw new DomainException("An active consultation cannot be cancelled");

            if (Status == AppointmentStatus.Cancelled)
                throw new DomainException("Appointment is already cancelled");

            Status = AppointmentStatus.Cancelled;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void Complete(string? notes = null, DateTime? completedAtUtc = null)
        {
            if (Status != AppointmentStatus.InConsultation)
                throw new DomainException("Only an active consultation can be completed");

            Status = AppointmentStatus.Completed;
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
            CompletedAtUtc = NormalizeUtc(completedAtUtc ?? DateTime.UtcNow);
            UpdatedAtUtc = DateTime.UtcNow;
        }

        private static DateTime NormalizeUtc(DateTime value)
            => value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
}
