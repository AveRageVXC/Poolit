namespace Poolit.Models;

public class FileEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreationDate { get; set; }
    public int Size { get; set; }
    public int OwnerId { get; set; }
    public string S3Key { get; set; }
    public string PoolitKey { get; set; }
}
