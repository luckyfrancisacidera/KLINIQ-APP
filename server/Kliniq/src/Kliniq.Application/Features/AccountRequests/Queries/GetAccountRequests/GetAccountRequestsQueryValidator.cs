using FluentValidation;

namespace Kliniq.Application.Features.AccountRequests.Queries.GetAccountRequests;

public sealed class GetAccountRequestsQueryValidator : AbstractValidator<GetAccountRequestsQuery>
{
    public GetAccountRequestsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Search).MaximumLength(200);
        RuleFor(x => x.Status)
            .Must(value => value is null || value.Equals("Pending", StringComparison.OrdinalIgnoreCase)
                || value.Equals("Approved", StringComparison.OrdinalIgnoreCase)
                || value.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Status must be Pending, Approved, or Rejected.");
    }
}
