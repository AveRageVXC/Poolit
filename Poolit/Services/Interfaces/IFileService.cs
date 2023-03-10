
using Poolit.Models;

namespace Poolit.Services.Interfaces;

public interface IFileService
{
    public string GetFileUrlById(ulong id);
    public bool FileExists(string path);
    public UserFile[] GetUserFiles(ulong userId);
}
