using Newtonsoft.Json;

namespace Poolit.Models;

public class FileEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreationDate { get; set; }
    public int Size { get; set; }
    [JsonIgnore]
    public int OwnerId { get; set; }
    [JsonIgnore]
    public string S3Key { get; set; }
    public string PoolitKey { get; set; }
    [JsonIgnore]
    public string ContentType { get; set; }
}
