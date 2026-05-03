namespace Kliniq.Application.Common.Models
{
    public record FileUpload
    {
        public Stream Content { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long Size { get; set; }
    }
}
