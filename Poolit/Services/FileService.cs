using Microsoft.Extensions.Options;
using Poolit.Models;
using System.IO;

namespace Poolit.Services;

public class FileService : IFileService
{
    public FileEntity[] GetAvailableFiles(int userId)
    {
        var userFiles = new FileEntity[] { new FileEntity{Id = 0, OwnerId = userId, Name = "1.png", Size = 10000, PoolitKey = "./1.png" } };
        return userFiles;
    }

    public void AddFile(FileEntity file, int ownerId, List<int> AccessEnabledUserId)
    {
        throw new NotImplementedException();
    }

    public FileEntity GetFileById(int fileId)
    {
        throw new NotImplementedException();
    }

    public FileEntity GetFileByPoolitKey(string poolitKey)
    {
        // Пиши репозитории, Глеб
        throw new NotImplementedException();
    }

    public bool DeleteFile(int fileId)
    {
        throw new NotImplementedException();
    }
}
