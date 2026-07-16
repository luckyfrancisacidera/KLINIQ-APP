using FluentValidation;

namespace Kliniq.Application.Features.Patients.Queries.GetPatients;

public sealed class GetPatientsQueryValidator : AbstractValidator<GetPatientsQuery>
{
    public GetPatientsQueryValidator()
    {
        RuleFor(query => query.Page).GreaterThan(0);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
        RuleFor(query => query.Search).MaximumLength(120);
    }
}
