namespace Optimus.Services.Customers.Application.Services;

public interface IFileManager
{
    Task<MemoryStream> GetFileStreamAsync(string fileName, string? directoryName = null);

    Task<bool> DirectoryExistsAsync(string directoryName);

    Task SaveFileAsync(
        string fileName,
        Stream stream,
        string directoryName = null,
        string contentType = null);

    Task MakeDirectoryAsync(string directoryName);

    Task<bool> DeleteFileAsync(string directoryName, string fileName);

    string GetUrl(string path, string? bucketName = null);

    bool IsImage(string contentType);
}