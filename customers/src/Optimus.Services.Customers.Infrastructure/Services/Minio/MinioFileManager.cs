using Optimus.Services.Customers.Application.Services;
using Microsoft.Extensions.Options;
using Minio;

namespace Optimus.Services.Customers.Infrastructure.Services.Minio;

public class MinioFileManager : IFileManager
{
  private readonly string[] _imageContentType =
    { "image/jpg", "image/jpeg", "image/pjpeg", "image/x-png", "image/png" };
  
    private readonly MinioClient _minio;
    private readonly MinioOptions _minioOptions;

    public MinioFileManager(IOptions<MinioOptions> minioOptions)
    {
      _minioOptions = minioOptions.Value;
      var connection = _minioOptions.MinioConnection;
      _minio = new MinioClient()
        .WithEndpoint(connection.Endpoint)
        .WithCredentials(connection.AccessKey, connection.SecretKey)
        .Build();
    }

    public async Task<MemoryStream> GetFileStreamAsync(string fileName, string? bucketName = null)
    {
      bucketName ??= _minioOptions.RootBucket;
      var ms = new MemoryStream();
      await _minio.GetObjectAsync(bucketName, fileName, (Action<Stream>) (s =>
      {
        s.CopyTo((Stream) ms);
        s.Close();
      }));
      ms.Position = 0L;
      return ms;
    }

    public async Task<bool> DirectoryExistsAsync(string bucketName) => await _minio.BucketExistsAsync(bucketName);

    public async Task SaveFileAsync(string filePath, Stream stream, string bucketName = null, string contentType = null)
    {
      var directoryExists = await DirectoryExistsAsync(_minioOptions.RootBucket);
      if (!directoryExists)
      {
        await MakeDirectoryAsync(_minioOptions.RootBucket);
      }

      if (string.IsNullOrEmpty(bucketName))
        bucketName = _minioOptions.RootBucket;
      else
        bucketName = _minioOptions + $"{bucketName}";
      
      await _minio.PutObjectAsync(bucketName, filePath, stream, stream.Length, contentType);
    }

    public async Task MakeDirectoryAsync(string bucketName) => await _minio.MakeBucketAsync(bucketName);

    public async Task<bool> DeleteFileAsync(string bucketName, string fileName)
    {
      await _minio.RemoveObjectAsync(bucketName, fileName);
      return true;
    }

    public string GetUrl(string path, string? bucketName = null)
    {
      if (bucketName is null)
        bucketName = _minioOptions.RootBucket;

      return $"{bucketName}/{path}";
    }

    public bool IsImage(string contentType)
    {
      return _imageContentType.Contains(contentType);
    }
}