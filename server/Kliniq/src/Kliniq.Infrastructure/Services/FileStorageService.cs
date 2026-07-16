using Kliniq.Application.Common.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;

namespace Kliniq.Infrastructure.Services
{
    public sealed class FileStorageService : IFileStorageService
    {
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".jpg", ".jpeg", ".png"
        };

        private readonly string _basePath;
        
        public FileStorageService(IConfiguration configuration)
        {
            var configuredPath = configuration["FileStorage:BasePath"];
            if (string.IsNullOrWhiteSpace(configuredPath))
                throw new InvalidOperationException("FileStorage:BasePath is not configured.");

            _basePath = Path.GetFullPath(configuredPath);
        }

        public async Task<string> UploadAsync(Stream fileStream, string originalFileName, string folder, CancellationToken cancellationToken)
        {
            var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new InvalidOperationException("The uploaded file extension is not allowed.");

            if (string.IsNullOrWhiteSpace(folder) || folder.Contains("..", StringComparison.Ordinal) || Path.IsPathRooted(folder))
                throw new InvalidOperationException("The storage folder is invalid.");

            var safeFileName = $"{Guid.NewGuid()}{extension}";

            var directory = Path.GetFullPath(Path.Combine(_basePath, folder));
            var rootPrefix = _basePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
            var isStorageRoot = string.Equals(directory, _basePath, StringComparison.OrdinalIgnoreCase);
            if (!isStorageRoot && !directory.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("The storage path is outside the configured root.");
            Directory.CreateDirectory(directory);

            var filePath = Path.Combine(directory, safeFileName);

            await using var stream = File.Create(filePath);
            await fileStream.CopyToAsync(stream, cancellationToken);
            
            return Path.Combine(folder, safeFileName).Replace("\\","/");

        }
    }
}
