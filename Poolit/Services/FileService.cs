using Poolit.Models;
using Poolit.Repositories;

namespace Poolit.Services;

public class FileService : IFileService
{
    private IFileRepo _fileRepo;
    private IUserRepo _userRepo;
    private Random _random;
    private const string ABC = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const int KeyLength = 7;

    public FileService(IFileRepo fileRepo, IUserRepo userRepo)
    {
        _userRepo = userRepo;
        _fileRepo = fileRepo;
        _random = new Random((int)DateTime.Now.Ticks);
    }

    public void SaveFile(FileEntity file, List<int> accessEnabledUserIds)
    {
        if (_userRepo.IdExists(file.OwnerId) is false)
        {
            throw new ArgumentException("Owner of this file doesn't exist!");
        }

        if (accessEnabledUserIds.All(userId => _userRepo.IdExists(userId)) is false)
        {
            throw new ArgumentException("User with access to the file doesn't exist!");
        }

        string s3Key;
        do
        {
            s3Key = GenerateString(KeyLength);
        } while (_fileRepo.ValidS3Key(s3Key) is false);

        string poolitKey;
        do
        {
            poolitKey = GenerateString(KeyLength);
        } while (_fileRepo.ValidPoolitKey(poolitKey) is false);

        file.S3Key = s3Key;
        file.PoolitKey = poolitKey;

        _fileRepo.SaveFile(file, accessEnabledUserIds);
    }

    private string GenerateString(int length)
        => new string(Enumerable.Repeat(ABC, length)
            .Select(s => s[_random.Next(ABC.Length)]).ToArray());

    public List<FileEntity> GetAvailableFiles(int userId)
    {
        return _fileRepo.GetAvailableFiles(userId);
    }


    public FileEntity GetFileById(int fileId)
    {
        return _fileRepo.GetFileById(fileId);
    }

    public FileEntity GetFileByPoolitKey(string poolitKey)
    {
        return _fileRepo.GetFileByPoolitKey(poolitKey);
    }
}
