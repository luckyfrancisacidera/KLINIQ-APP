using Kliniq.Domain.Common;
using Kliniq.Domain.Entities;
using Kliniq.Domain.Enums;
using Kliniq.Domain.ValueObjects;

public class AccountRequestTests
{
    [Fact]
    public void Approve_WhenPending_SetsStatusToApproved()
    {
        var request = CreateValidAccountRequest();

        request.Approve("Looks good");

        Assert.Equal(AccountRequestStatus.Approved, request.Status);
        Assert.NotNull(request.InvitationToken);
        Assert.NotNull(request.InvitationExpiresAt);
    }

    [Fact]
    public void Approve_WhenAlreadyApproved_ThrowsDomainException()
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
    public void Reject_WhenAlreadyRejected_ThrowsDomainException()
    {
        var request = CreateValidAccountRequest();
        request.Reject("reason");

        Assert.Throws<DomainException>(() => request.Reject("another reason"));
    }

    [Fact]
    public void MarkInvitationUsed_WhenAlreadyUsed_ThrowsDomainException()
    {
        var request = CreateValidAccountRequest();
        request.Approve();
        request.MarkInvitationUsed();

        Assert.Throws<DomainException>(() => request.MarkInvitationUsed());
    }

    [Fact]
    public void InvitationToken_IsGeneratedOnApproval()
    {
        var request = CreateValidAccountRequest();
        request.Approve();

        Assert.NotNull(request.InvitationToken);
        Assert.False(request.IsInvitationUsed);
        Assert.True(request.InvitationExpiresAt > DateTime.UtcNow);
    }

    private static AccountRequest CreateValidAccountRequest() => new(
        new FullName("Juan", "Dela Cruz"),
        "juan@email.com",
        "Cardiology",
        new Address("123 Main St", "Manila", "Philippines"),
        "prc-path",
        "board-cert-path",
        "diploma-path",
        "good-standing-path");
}