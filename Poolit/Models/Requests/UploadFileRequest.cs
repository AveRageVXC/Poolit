namespace Poolit.Models.Requests;

public class UploadFileRequest
{
    public IFormFile File { get; set; }
    public ulong Id { get; set; }
}
