using Poolit.Models;

namespace Poolit.Services;

public interface IFileService
{
    // Внутри создаётся ранд poolitKey и s3 key
    void AddFile(FileEntity file, int ownerId, List<int> AccessEnabledUserId);
    FileEntity[] GetAvailableFiles(int userId);
    FileEntity GetFileById(int fileId);
    FileEntity GetFileByPoolitKey(string poolitKey);
    bool DeleteFile(int fileId);
}
