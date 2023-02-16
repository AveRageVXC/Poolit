using Microsoft.AspNetCore.Mvc;

namespace Poolit.Controllers;

public class ConfigurationDemoController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public ConfigurationDemoController(IConfiguration configuration)
    {   
        _configuration = configuration;
    }
    [HttpGet("AWS/access")]
    public string GetAccessKey()
    {
        var access_key = _configuration.GetValue<string>("AWS:access");
        return access_key;
    }
}