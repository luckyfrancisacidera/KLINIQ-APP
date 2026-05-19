namespace Kliniq.Application.Common.Interfaces.Repositories
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(
            Stream fileStream,
            string originalFileName,
            string folder,
            CancellationToken cancellationToken
            );
    }
}
