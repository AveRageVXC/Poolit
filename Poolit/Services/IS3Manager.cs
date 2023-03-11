using Amazon.S3.Model;

namespace Poolit.Services;

public interface IS3Manager
{
    Task PutFileAsync(Stream fileStream, string key);
    Task<GetObjectResponse> GetObjectAsync(string key);
}