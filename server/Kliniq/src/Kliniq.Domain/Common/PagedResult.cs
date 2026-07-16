using System.Text.Json.Serialization;

namespace Kliniq.Domain.Common
{
    public sealed class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; }
        public int Page { get; }
        public int PageSize { get; }
        public int TotalItems { get; }
        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalItems / PageSize);
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;

        [JsonIgnore]
        public int TotalCount => TotalItems;

        public PagedResult(IReadOnlyList<T> items, int totalItems, int page, int pageSize)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
            TotalItems = Math.Max(0, totalItems);
            Page = Math.Max(1, page);
            PageSize = Math.Clamp(pageSize, 1, 100);
        }
    }
}
