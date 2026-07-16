using Kliniq.Domain.Common;
using Kliniq.Domain.Entities;
using Kliniq.Domain.Enums;

namespace Kliniq.Domain.Tests;

public sealed class AppointmentTests
{
    [Fact]
    public void Constructor_WithPastTime_Throws()
    {
        Assert.Throws<ArgumentException>(() => Create(DateTime.UtcNow.AddMinutes(-1)));
    }

    [Fact]
    public void FullClinicWorkflow_ConfirmedQueuedConsultationCompleted_FollowsAllowedTransitions()
    {
        var appointment = Create(DateTime.UtcNow.AddDays(1));
        var queuedAt = DateTime.UtcNow.AddMinutes(1);
        var startedAt = DateTime.UtcNow.AddMinutes(5);
        var completedAt = DateTime.UtcNow.AddMinutes(25);

        appointment.Confirm();
        appointment.JoinQueue(queuedAt);
        appointment.StartConsultation(startedAt);
        appointment.Complete("Visit completed", completedAt);

        Assert.Equal(AppointmentStatus.Completed, appointment.Status);
        Assert.Equal(queuedAt, appointment.QueuedAtUtc);
        Assert.Equal(startedAt, appointment.ConsultationStartedAtUtc);
        Assert.Equal(completedAt, appointment.CompletedAtUtc);
        Assert.Equal("Visit completed", appointment.Notes);
    }

    [Fact]
    public void Complete_WhenNotInConsultation_ThrowsDomainException()
    {
        var appointment = Create(DateTime.UtcNow.AddDays(1));
        appointment.Confirm();
        Assert.Throws<DomainException>(() => appointment.Complete());
    }

    [Fact]
    public void StartConsultation_WhenNotQueued_ThrowsDomainException()
    {
        var appointment = Create(DateTime.UtcNow.AddDays(1));
        appointment.Confirm();
        Assert.Throws<DomainException>(() => appointment.StartConsultation(DateTime.UtcNow));
    }

    [Fact]
    public void Reschedule_QueuedAppointment_ThrowsDomainException()
    {
        var appointment = Create(DateTime.UtcNow.AddDays(1));
        appointment.Confirm();
        appointment.JoinQueue(DateTime.UtcNow);

        Assert.Throws<DomainException>(() => appointment.Reschedule(DateTime.UtcNow.AddDays(2), TimeSpan.FromMinutes(30)));
    }

    [Fact]
    public void Reschedule_ValidSlot_UpdatesEndTimeAndReturnsToPending()
    {
        var appointment = Create(DateTime.UtcNow.AddDays(1));
        appointment.Confirm();
        var next = DateTime.UtcNow.AddDays(2);

        appointment.Reschedule(next, TimeSpan.FromMinutes(45));

        Assert.Equal(AppointmentStatus.Pending, appointment.Status);
        Assert.Equal(next, appointment.ScheduledAt);
        Assert.Equal(next.AddMinutes(45), appointment.EndTime);
    }

    private static Appointment Create(DateTime scheduledAt) => new(
        Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), scheduledAt, TimeSpan.FromMinutes(30), "Consultation");
}
