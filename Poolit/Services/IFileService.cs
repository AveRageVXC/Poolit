using Poolit.Models;

namespace Poolit.Services;

public interface IFileService
{
    // Внутри создаётся ранд poolitKey и s3 key
    void SaveFile(FileEntity file, List<int> accessEnabledUserIds);
    List<FileEntity> GetAvailableFiles(int userId);
    FileEntity GetFileById(int fileId);
    FileEntity GetFileByPoolitKey(string poolitKey);
    bool DeleteFile(int fileId);
}
