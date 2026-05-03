namespace Kliniq.Application.Common.Interfaces.Repositories
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(
            Stream fileStream,
            string fileName,
            string folder,
            CancellationToken cancellationToken
            );
    }
}
