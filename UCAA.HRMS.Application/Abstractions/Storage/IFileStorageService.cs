namespace UCAA.HRMS.Application.Abstractions.Storage;

public interface IFileStorageService
{
    Task<string> SaveAsync(Stream content, string originalFileName, CancellationToken cancellationToken = default);
    Task<Stream> OpenReadAsync(string storedFileName, CancellationToken cancellationToken = default);
}
