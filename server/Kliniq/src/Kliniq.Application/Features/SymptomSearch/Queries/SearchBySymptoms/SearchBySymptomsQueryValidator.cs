using FluentValidation;

namespace Kliniq.Application.Features.SymptomSearch.Queries.SearchBySymptoms
{
    public sealed class SearchBySymptomsQueryValidator : AbstractValidator<SearchBySymptomsQuery>
    {
        public SearchBySymptomsQueryValidator()
        {
            RuleFor(request => request.Symptoms)
                .NotEmpty()
                .MinimumLength(10)
                .MaximumLength(1500);

            RuleFor(request => request.Page).GreaterThan(0);
            RuleFor(request => request.PageSize).InclusiveBetween(1, 20);
        }
    }
}
