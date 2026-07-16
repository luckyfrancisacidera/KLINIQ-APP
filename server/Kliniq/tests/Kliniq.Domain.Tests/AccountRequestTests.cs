using Kliniq.Domain.Common;
using Kliniq.Domain.Entities;
using Kliniq.Domain.Enums;
using Kliniq.Domain.ValueObjects;

namespace Kliniq.Domain.Tests;

public sealed class AccountRequestTests
{
    [Fact]
    public void Approve_WhenPending_SetsStatusAndInvitation()
    {
        var request = CreateValidAccountRequest();

        request.Approve("Credentials reviewed");

        Assert.Equal(AccountRequestStatus.Approved, request.Status);
        Assert.NotNull(request.InvitationToken);
        Assert.True(request.InvitationExpiresAt > DateTime.UtcNow);
        Assert.False(request.IsInvitationUsed);
    }

    [Fact]
    public void Approve_WhenAlreadyProcessed_ThrowsDomainException()
    {
        var request = CreateValidAccountRequest();
        request.Approve();

        Assert.Throws<DomainException>(() => request.Approve());
    }

    [Fact]
    public void Reject_WithoutAdminNote_ThrowsDomainException()
    {
        var request = CreateValidAccountRequest();
        Assert.Throws<DomainException>(() => request.Reject(null));
    }

    [Fact]
    public void MarkInvitationUsed_WhenAlreadyUsed_ThrowsDomainException()
    {
        var request = CreateValidAccountRequest();
        request.Approve();
        request.MarkInvitationUsed();

        Assert.Throws<DomainException>(() => request.MarkInvitationUsed());
    }

    private static AccountRequest CreateValidAccountRequest() => new(
        new FullName("Juan", "Dela Cruz"),
        "juan@example.com",
        "011-123456",
        ["Cardiology"],
        new Address("123 Main St", "Laoag City", "Philippines"),
        "KLINIQ Medical Center",
        new GeoLocation(18.1987, 120.5936),
        "prc-license.pdf",
        "government-id.pdf",
        "professional-photo.jpg",
        "cv.pdf");
}
