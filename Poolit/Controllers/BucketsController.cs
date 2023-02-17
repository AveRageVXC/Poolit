using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/buckets")]
public class BucketsController : ControllerBase
{
    public static AmazonS3Client S3Client;
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateBucketAsync(string bucketName)
    {
        await S3Client.PutBucketAsync(bucketName);
        return Ok($"Bucket {bucketName} created.");
    }
}



