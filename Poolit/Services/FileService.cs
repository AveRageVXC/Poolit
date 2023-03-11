using Microsoft.Extensions.Options;
using Poolit.Models;
using System.IO;

namespace Poolit.Services;

public class FileService : IFileService
{
    public string GetFileUrlById(int id)
    {
        // Опа, из репозитория достается ссылка на файл по его id.
        var fileName = $"{id}.png";
        var url = $"./{fileName}";
        return url;
    }

    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public UserFile[] GetUserFiles(int userId)
    {
        var userFiles = new UserFile[] { new UserFile{Id = 0, AuthorId = userId, Name = "1.png", Size = 10000, Url = "./1.png" } };
        return userFiles;
    }
}
