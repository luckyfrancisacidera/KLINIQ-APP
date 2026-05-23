namespace Kliniq.Application.Features.AccountRequests.DTOs
{
    public record AccountRequestSummaryDto
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public IReadOnlyList<string> Specializations { get; init; } = [];
        public string Street { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public DateTime CreatedAtUtc { get; init; }
    }
}
