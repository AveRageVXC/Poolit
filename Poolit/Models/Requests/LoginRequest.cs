namespace Poolit.Models.Requests;

public record LoginRequest
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string Token { get; set; }
}
