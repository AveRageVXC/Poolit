using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/buckets")]
public class BucketsController : ControllerBase
{
    public static AmazonS3Client S3Client;
    /*private readonly IAmazonS3 _s3Client;
    public  BucketsController(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }*/
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateBucketAsync(string bucketName)
    {
        /*var bucketExists = await S3Client.DoesS3BucketExistAsync(bucketName);
        if (bucketExists) return BadRequest($"Bucket {bucketName} already exists.");*/
        await S3Client.PutBucketAsync(bucketName);
        return Ok($"Bucket {bucketName} created.");
    }
    /*private readonly IAmazonS3 _s3Client;
    public BucketsController(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateBucketAsync(string bucketName)
    {
        var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
        if (bucketExists) return BadRequest($"Bucket {bucketName} already exists.");
        await _s3Client.PutBucketAsync(bucketName);
        return Ok($"Bucket {bucketName} created.");
    }*/
    /*
    public static async Task<bool> CreateBucketAsync(IAmazonS3 client, string bucketName)
    {
        try
        {
            var request = new PutBucketRequest
            {
                BucketName = bucketName,
                UseClientRegion = true,
            };

            var response = await client.PutBucketAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine($"Error creating bucket: '{ex.Message}'");
            return false;
        }
    }*/
}



