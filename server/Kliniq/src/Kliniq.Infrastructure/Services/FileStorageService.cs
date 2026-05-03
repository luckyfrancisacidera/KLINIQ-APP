using Kliniq.Application.Common.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;

namespace Kliniq.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _basePath;
        
        public FileStorageService(IConfiguration configuration)
        {
            var relativePath = configuration["FileStorage:BasePath"] ?? throw new InvalidOperationException("FileStorage:BasePath is not configured");
        }

        public async Task<string> UplaodAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken)
        {
            var directory = Path.Combine(_basePath, fileName);
            Directory.CreateDirectory(directory);

            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var filePath = Path.Combine(directory, uniqueFileName);

            using var stream = File.Create(filePath);
            await fileStream.CopyToAsync(stream, cancellationToken);
            
            return Path.Combine(folder, uniqueFileName);

        }
    }
}
