using Kliniq.Domain.Common;
using Kliniq.Domain.Enums;

namespace Kliniq.Domain.Entities
{
    public class Appointment : AuditableEntity
    {
        public Guid PatientId { get; private set; }
        public Guid DoctorId { get; private set; }
        public Guid ClinicId { get; private set; }

        public  DateTime ScheduledAt { get; private set; }
        public TimeSpan Duration { get; private set; }
        public AppointmentStatus Status { get; private set; }
        public string? Reason { get; private set; }

        private Appointment() { }

        public Appointment(
            Guid patientId,
            Guid doctorId,
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
            DoctorId = doctorId;
            ClinicId = clinicId;
            ScheduledAt = scheduledAt;
            Duration = duration;
            Status = AppointmentStatus.Pending;
            Reason = reason;

        }

        public void Confirm()
        {
            if(Status != AppointmentStatus.Pending)
                throw new DomainException("Only pending appointments can be confirmed");

            Status = AppointmentStatus.Confirmed;
        }

        public void Cancel()
        {
            if (Status == AppointmentStatus.Completed)
                throw new DomainException("Cannot cancel completed appointment");
            Status = AppointmentStatus.Cancelled;
        }

        public void Complete()
        {
            if(Status != AppointmentStatus.Confirmed)
                throw new DomainException("Only confirmed appointments can be completed");
            Status = AppointmentStatus.Completed;
        }


    }
}
