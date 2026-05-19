namespace Kliniq.Application.Common.Models
{
    public record FileUpload
    {
        public Stream Content { get; init; } = null!;
        public string FileName { get; init; } = string.Empty;
        public string ContentType { get; init; } = string.Empty;
        public long Size { get; init; }

        public string Extension => Path.GetExtension(FileName).ToLowerInvariant();
        public string NormalizedContentType => ContentType.ToLowerInvariant();
    }
}
