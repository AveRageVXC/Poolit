namespace Poolit.Models.Responses;

public class RegisterResponse
{
    public User User { get; set; }
    public bool HasError { get; set; }
    public string Error { get; set; }
}
