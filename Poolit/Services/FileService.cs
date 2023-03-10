using Microsoft.Extensions.Options;
using Poolit.Models.Requests;
using Poolit.Models.Responses;
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

    public async Task<DownloadFileResponse> DownloadAsync(DownloadFileRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<GetFileDataResponse> GetFileDataAsync(GetFileDataRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<GetUserFilesResponse> GetUserFilesAsync(GetUserFilesRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<UploadFileResponse> UploadAsync(UploadFileRequest request)
    {
        var file = request.File;
        string path = "./" + file.FileName;
        using (var fileStream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }
        var response = new UploadFileResponse
        {
            Url = path
        };
        return response;
    }
}
