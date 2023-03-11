using Newtonsoft.Json;

namespace Poolit.Models;

public class User
{
    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonProperty("login")]
    public string Login { get; set; }
    [JsonIgnore]
    public string HashedPassword { get; set; }
    [JsonProperty("token")]
    public string Token { get; set; }
}
