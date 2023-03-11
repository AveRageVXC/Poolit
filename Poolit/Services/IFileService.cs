using Poolit.Models;

namespace Poolit.Services;

public interface IFileService
{
    public string GetFileUrlById(int id);
    public bool FileExists(string path);
    public UserFile[] GetUserFiles(int userId);
}
