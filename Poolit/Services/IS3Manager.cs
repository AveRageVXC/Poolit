using Amazon.S3.Model;

namespace Poolit.Services;

public interface IS3Manager
{
    public Task PutObjectAsync(Stream file, string key);
    public Task<GetObjectResponse> GetObjectAsync(string key);
}