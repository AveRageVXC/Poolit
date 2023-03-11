namespace Poolit.Models;

[Serializable]
public class User
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string HashedPassword { get; set; }
    public string Token { get; set; }
}
