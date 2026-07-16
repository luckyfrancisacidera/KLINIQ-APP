namespace Kliniq.Application.Common.Models
{
    public static class Pagination
    {
        public const int DefaultPage = 1;
        public const int DefaultPageSize = 20;
        public const int MaximumPageSize = 100;

        public static (int Page, int PageSize) Normalize(
            int page,
            int pageSize)
        {
            var normalizedPage = page < DefaultPage
                ? DefaultPage
                : page;

            var normalizedPageSize = pageSize <= 0
                ? DefaultPageSize
                : Math.Min(pageSize, MaximumPageSize);

            return (normalizedPage, normalizedPageSize);
        }
    }
}