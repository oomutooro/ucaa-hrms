using UCAA.HRMS.Application.Abstractions.Storage;

namespace UCAA.HRMS.Infrastructure.Storage;

public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly string _root;

    public LocalFileStorageService(string root)
    {
        _root = root;
        Directory.CreateDirectory(_root);
    }

    public async Task<string> SaveAsync(Stream content, string originalFileName, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(originalFileName);
        var storedName = $"{Guid.NewGuid()}{extension}";
        var path = Path.Combine(_root, storedName);

        await using var fileStream = File.Create(path);
        content.Position = 0;
        await content.CopyToAsync(fileStream, cancellationToken);
        return storedName;
    }

    public Task<Stream> OpenReadAsync(string storedFileName, CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(_root, storedFileName);
        Stream stream = File.OpenRead(path);
        return Task.FromResult(stream);
    }
}
