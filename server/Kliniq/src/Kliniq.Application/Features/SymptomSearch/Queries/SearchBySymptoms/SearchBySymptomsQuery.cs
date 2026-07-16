using Kliniq.Application.Common.Security;
using Kliniq.Application.Features.SymptomSearch.DTOs;
using Kliniq.Domain.Common;
using MediatR;

namespace Kliniq.Application.Features.SymptomSearch.Queries.SearchBySymptoms
{
    public sealed record SearchBySymptomsQuery(
        string Symptoms,
        int Page = 1,
        int PageSize = 6) : IRequest<Result<SymptomSearchResponseDto>>, ISensitiveRequest;
}
