using FluentValidation;

namespace Kliniq.Application.Features.Practitioners.Queries.GetPractitioners
{
    public sealed class GetPractitionersQueryValidator : AbstractValidator<GetPractitionersQuery>
    {
        public GetPractitionersQueryValidator()
        {
            RuleFor(x => x.Page).GreaterThan(0);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
            RuleFor(x => x.Search).MaximumLength(150);
            RuleFor(x => x.Specialization).MaximumLength(100);
        }
    }
}
