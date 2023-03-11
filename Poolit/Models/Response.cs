using Newtonsoft.Json;

namespace Poolit.Models;

public class Response
{
    private IDataEntry[] _data;

    [JsonProperty("data")]
    public IDataEntry[] Data
    {
        get => _data;
        set
        {
            _data = value;
            if (_data.Length > 0)
            {
                for (var i = 0; i < _data.Length; i++)
                {
                    _data[i].Id = (ulong)i + 1;
                }
            }
        }
    }
    [JsonProperty("error")]
    public string Error { get; set; }
}
