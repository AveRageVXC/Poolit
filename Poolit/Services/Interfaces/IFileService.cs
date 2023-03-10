using Poolit.Models.Requests;
using Poolit.Models.Responses;

namespace Poolit.Services.Interfaces;

public interface IFileService
{
    public Task<UploadFileResponse> UploadAsync(UploadFileRequest request);
    public Task<DownloadFileResponse> DownloadAsync(DownloadFileRequest request);
    public Task<GetUserFilesResponse> GetUserFilesAsync(GetUserFilesRequest request);
    public Task<GetFileDataResponse> GetFileDataAsync(GetFileDataRequest request);
}
