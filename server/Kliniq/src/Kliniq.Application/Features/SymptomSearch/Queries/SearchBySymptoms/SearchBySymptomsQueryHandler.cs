using Kliniq.Application.Common.Interfaces;
using Kliniq.Application.Common.Interfaces.Repositories;
using Kliniq.Application.Features.SymptomSearch.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.SymptomSearch.Queries.SearchBySymptoms
{
    public sealed class SearchBySymptomsQueryHandler : IRequestHandler<SearchBySymptomsQuery, Result<SymptomSearchResponseDto>>
    {
        private const string Disclaimer = "KLINIQ provides physician-matching guidance, not a diagnosis or treatment plan. Recommendations are generated from the words you provide and may be incomplete. For emergencies, seek immediate in-person care.";

        private readonly ISymptomAnalysisService _analysisService;
        private readonly IPractitionerRepository _practitionerRepository;

        public SearchBySymptomsQueryHandler(
            ISymptomAnalysisService analysisService,
            IPractitionerRepository practitionerRepository)
        {
            _analysisService = analysisService;
            _practitionerRepository = practitionerRepository;
        }

        public async Task<Result<SymptomSearchResponseDto>> Handle(SearchBySymptomsQuery request, CancellationToken cancellationToken)
        {
            var analysis = _analysisService.Analyze(request.Symptoms);
            if (analysis.Urgency.Equals("Emergency", StringComparison.OrdinalIgnoreCase))
            {
                return Result.Success(new SymptomSearchResponseDto(
                    analysis.Urgency,
                    analysis.Guidance,
                    analysis.SpecialtyMatches.Select(match => new SpecialtySuggestionDto(
                        match.Specialty,
                        match.MatchScore,
                        match.MatchedSignals)).ToList(),
                    new PagedResult<SuggestedPractitionerDto>([], 0, request.Page, request.PageSize),
                    Disclaimer));
            }

            var primarySpecialty = analysis.SpecialtyMatches[0].Specialty;

            var practitioners = await _practitionerRepository.SearchAsync(
                search: null,
                specialization: NormalizeSpecialtyForSearch(primarySpecialty),
                page: request.Page,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken);
            var usedGeneralPracticeFallback = false;

            if (practitioners.TotalCount == 0 && !primarySpecialty.Equals("General Practice", StringComparison.OrdinalIgnoreCase))
            {
                practitioners = await _practitionerRepository.SearchAsync(
                    search: null,
                    specialization: "General",
                    page: request.Page,
                    pageSize: request.PageSize,
                    cancellationToken: cancellationToken);
                usedGeneralPracticeFallback = practitioners.TotalCount > 0;
            }

            var practitionerDtos = practitioners.Items.Select(practitioner =>
            {
                var specializations = ParseSpecializations(practitioner.SpecializationsRaw);
                var matched = analysis.SpecialtyMatches.FirstOrDefault(match =>
                    specializations.Any(spec => SpecialtyMatches(spec, match.Specialty)));

                return new SuggestedPractitionerDto(
                    practitioner.Id,
                    practitioner.Name.FirstName,
                    practitioner.Name.LastName,
                    practitioner.LicenseNumber,
                    specializations,
                    practitioner.ClinicID,
                    practitioner.Clinic?.Name,
                    matched?.MatchScore ?? (usedGeneralPracticeFallback ? 35 : analysis.SpecialtyMatches[0].MatchScore));
            }).ToList();

            var response = new SymptomSearchResponseDto(
                analysis.Urgency,
                analysis.Guidance,
                analysis.SpecialtyMatches.Select(match => new SpecialtySuggestionDto(
                    match.Specialty,
                    match.MatchScore,
                    match.MatchedSignals)).ToList(),
                new PagedResult<SuggestedPractitionerDto>(
                    practitionerDtos,
                    practitioners.TotalCount,
                    practitioners.Page,
                    practitioners.PageSize),
                Disclaimer);

            return Result.Success(response);
        }

        private static string NormalizeSpecialtyForSearch(string specialty)
            => specialty switch
            {
                "Otolaryngology (ENT)" => "ENT",
                "Psychiatry or Psychology" => "Psych",
                "Obstetrics and Gynecology" => "Gyne",
                "Emergency Medicine" => "Emergency",
                _ => specialty,
            };

        private static IReadOnlyList<string> ParseSpecializations(string raw)
            => raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(value => value.Trim('[', ']', '"', '\'', ' '))
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToList();

        private static bool SpecialtyMatches(string practitionerSpecialty, string suggestedSpecialty)
        {
            var practitioner = practitionerSpecialty.ToLowerInvariant();
            var suggested = NormalizeSpecialtyForSearch(suggestedSpecialty).ToLowerInvariant();
            return practitioner.Contains(suggested) || suggested.Contains(practitioner);
        }
    }
}
