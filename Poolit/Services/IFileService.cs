using Poolit.Models;

namespace Poolit.Services;

public interface IFileService
{
    // Внутри создаётся ранд poolitKey и s3 key
    void AddFile(FileEntity file, int ownerId, List<int> AccessEnabledUserId);
    List<FileEntity> GetAvailableFiles(User user);
    FileEntity GetFileById(int fileId);
    FileEntity GetFileByPoolitKey(string poolitKey);
}
