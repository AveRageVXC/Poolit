using Microsoft.Extensions.Options;
using Poolit.Models;
using System.IO;
using Poolit.Repositories;

namespace Poolit.Services;

public class FileService : IFileService
{
    private IFileRepo _fileRepo;
    private IUserRepo _userRepo;
    private Random _random;
    private const string ABC = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public FileService(IFileRepo fileRepo, IUserRepo userRepo)
    {
        _userRepo = userRepo;
        _fileRepo = fileRepo;
        _random = new Random(DateTime.Now.Millisecond);
    }

    public void SaveFile(FileEntity file, List<int> accessEnabledUserIds)
    {
        if (_userRepo.IdExists(file.OwnerId) is false)
        {
            throw new ArgumentException("Не существует владельца файла!");
        }

        if (accessEnabledUserIds.All(userId => _userRepo.IdExists(userId)) is false)
        {
            throw new ArgumentException("В списке, кому доступен файл, указан не существующий пользователь!");
        }

        string S3Key;
        do
        {
            S3Key = GenerateString(10);
        } while (_fileRepo.ValidS3Key(S3Key));

        string PoolitKey;
        do
        {
            PoolitKey = GenerateString(10);
        } while (_fileRepo.ValidPoolitKey(PoolitKey));

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
