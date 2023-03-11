using Microsoft.Extensions.Options;
using Poolit.Models;
using Poolit.Services.Interfaces;
using System.IO;

namespace Poolit.Services;

public class FileService : IFileService
{
    private IOptions<TokensConfiguration> _tokenConfiguration;

    public FileService(IOptions<TokensConfiguration> tokenConfiguration)
    {
        _tokenConfiguration = tokenConfiguration;
    }

    public string GetFileUrlById(ulong id)
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

    public UserFile[] GetUserFiles(ulong userId)
    {
        var userFiles = new UserFile[] { new UserFile{Id = 0, AuthorId = userId, Name = "1.png", Size = 10000, Url = "./1.png" } };
        return userFiles;
    }
}
