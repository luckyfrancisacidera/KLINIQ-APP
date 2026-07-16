using Kliniq.Application.Common.Models;

namespace Kliniq.Application.Tests;

public sealed class PaginationTests
{
    [Theory]
    [InlineData(0, 0, 1, 20)]
    [InlineData(2, 500, 2, 100)]
    [InlineData(4, 25, 4, 25)]
    public void Normalize_ConstrainsPageAndPageSize(int page, int pageSize, int expectedPage, int expectedPageSize)
    {
        var actual = Pagination.Normalize(page, pageSize);
        Assert.Equal(expectedPage, actual.Page);
        Assert.Equal(expectedPageSize, actual.PageSize);
    }
}
