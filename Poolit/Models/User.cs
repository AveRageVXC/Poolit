using Newtonsoft.Json;

namespace Poolit.Models;

public class User
{
    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonProperty("username")]
    public string Username { get; set; }
    [JsonIgnore]
    public string HashedPassword { get; set; }
}
