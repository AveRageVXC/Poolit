using Newtonsoft.Json;

namespace Poolit.Models;

public class FileEntity
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    // [JsonProperty("real_file_name")]
    [JsonIgnore]
    public string RealFileName { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("creation_date")]
    public DateTime CreationDate { get; set; }

    [JsonProperty("size")]
    public int Size { get; set; }

    [JsonIgnore]
    public int OwnerId { get; set; }
    
    [JsonIgnore]
    public string ContentType { get; set; } = string.Empty;

    [JsonIgnore]
    public string S3Key { get; set; } = string.Empty;

    [JsonProperty("poolit_key")]
    public string PoolitKey { get; set; } = string.Empty;
}
