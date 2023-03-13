namespace Poolit.Models.Requests;

public class UploadRequest
{
    public int UserId { get; set; }
    public IFormFile FormFile { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<int> AccessEnabledUserIds { get; set; }
}
