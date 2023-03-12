using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Poolit.Configurations;

namespace Poolit.Services;

public class S3Manager : IS3Manager
{
    private string BucketName;
    private AmazonS3Client S3Client;

    public S3Manager(IOptions<S3Configuration> S3Configuration)
    {
        AmazonS3Config config = new AmazonS3Config()
        {
            ServiceURL = "http://s3.yandexcloud.net:80",
            UseHttp = true,
            ForcePathStyle = true,
            ProxyHost = "s3.yandexcloud.net",
            ProxyPort = 80
        };
        var a = S3Configuration;
        AWSCredentials creds =
            new BasicAWSCredentials(S3Configuration.Value.AccessKey, S3Configuration.Value.SecretKey);
        S3Client = new AmazonS3Client(creds, config);
        BucketName = S3Configuration.Value.BucketName;
        CheckOrCreateBucketAsync();
    }

    public async Task CheckOrCreateBucketAsync()
    {
        ListBucketsResponse response = await S3Client.ListBucketsAsync();
        if (response.Buckets.Select(x => x.BucketName).Contains(BucketName) is false)
        {
            S3Client.PutBucketAsync(BucketName);
        }
    }

    public async Task PutFileAsync(Stream file, string key)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = BucketName,
            Key = key,
            InputStream = file
        };
        await S3Client.PutObjectAsync(putObjectRequest);
    }

    /*public async Task<ListBucketsResponse> GetListOfBucketsAsync()
    {
        ListBucketsResponse response = await S3Client.ListBucketsAsync();
        return response;
    }*/

    /*public async Task PutObjectAsync(string key, string text)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = this.BucketName,
            Key = key,
            ContentBody = text
        };
        await S3Client.PutObjectAsync(putObjectRequest);
        Console.WriteLine("Положили объект");
    }*/

    /*public async Task PutObjectAsync(string key, string filePath, MetadataCollection metadataCollection)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = BucketName,
            Key = key,
            FilePath = filePath
        };
        await S3Client.PutObjectAsync(putObjectRequest);
        Console.WriteLine("Положили объект");
    }*/

    public async Task<GetObjectResponse> GetFileAsync(string key)
    {
        var getObjectRequest = new GetObjectRequest
        {
            BucketName = BucketName,
            Key = key,
        };
        var res = await S3Client.GetObjectAsync(getObjectRequest);
        return res;
    }
}