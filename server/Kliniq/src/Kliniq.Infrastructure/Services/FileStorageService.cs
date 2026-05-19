using Kliniq.Application.Common.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;

namespace Kliniq.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _basePath;
        
        public FileStorageService(IConfiguration configuration)
        {
            _basePath = configuration["FileStorage:BasePath"] ?? throw new InvalidOperationException("FileStorage:BasePath is not configured");
        }

        public async Task<string> UploadAsync(Stream fileStream, string originalFileName, string folder, CancellationToken cancellationToken)
        {
            var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
            var safeFileName = $"{Guid.NewGuid()}{extension}";

            var directory = Path.Combine(_basePath, folder);
            Directory.CreateDirectory(directory);

            var filePath = Path.Combine(directory, safeFileName);

            await using var stream = File.Create(filePath);
            await fileStream.CopyToAsync(stream, cancellationToken);
            
            return Path.Combine(folder, safeFileName).Replace("\\","/");

        }
    }
}
