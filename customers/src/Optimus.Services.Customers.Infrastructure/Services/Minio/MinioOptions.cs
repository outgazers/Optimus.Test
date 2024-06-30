namespace Optimus.Services.Customers.Infrastructure.Services.Minio;

public class MinioOptions
{
    public MinioConnection MinioConnection { get; set; }

    public string RootBucket { get; set; }
}

public class MinioConnection
{
    public string Endpoint { get; set; }

    public string AccessKey { get; set; }

    public string SecretKey { get; set; }
}