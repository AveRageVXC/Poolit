using Poolit.Models;

namespace Poolit.Repositories;

public interface IFileRepo
{
    bool ValidS3Key(string S3Key);
    bool ValidPoolitKey(string PoolitKey);
    void SaveFile(FileEntity file, List<int> accessEnabledUserIds);
    List<FileEntity> GetAvailableFiles(int userId);
    FileEntity GetFileById(int fileId);
    FileEntity GetFileByPoolitKey(string poolitKey);
}