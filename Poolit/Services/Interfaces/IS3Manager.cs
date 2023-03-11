namespace Poolit.Services.Interfaces;

public interface IS3Manager
{
    public Task PutObjectAsync(Stream file, string key);
}