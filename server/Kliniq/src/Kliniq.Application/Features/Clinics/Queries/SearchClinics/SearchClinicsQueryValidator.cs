using FluentValidation;

namespace Kliniq.Application.Features.Clinics.Queries.SearchClinics
{
    public sealed class SearchClinicsQueryValidator : AbstractValidator<SearchClinicsQuery>
    {
        public SearchClinicsQueryValidator()
        {
            RuleFor(x => x.Page).GreaterThan(0);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
            RuleFor(x => x.Latitude).InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue);
            RuleFor(x => x.Longitude).InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue);
            RuleFor(x => x.RadiusKm).InclusiveBetween(1, 200).When(x => x.RadiusKm.HasValue);
            RuleFor(x => x)
                .Must(x => x.Latitude.HasValue == x.Longitude.HasValue)
                .WithMessage("Latitude and longitude must be supplied together.");
        }
    }
}
