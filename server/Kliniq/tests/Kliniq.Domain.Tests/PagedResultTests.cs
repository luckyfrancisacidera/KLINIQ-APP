using Kliniq.Domain.Common;

namespace Kliniq.Domain.Tests;

public sealed class PagedResultTests
{
    [Fact]
    public void CalculatesNavigationMetadata()
    {
        var result = new PagedResult<int>([21, 22], 22, 3, 10);

        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
        Assert.Equal(22, result.TotalItems);
    }
}
