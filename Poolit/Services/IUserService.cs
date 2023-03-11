using Microsoft.AspNetCore.Identity;
using Poolit.Models;

namespace Poolit.Services;

public interface IUserService
{
    public string HashPassword(User user, string password);
    public bool VerifyPassword(User user, string hashedPassword, string password);
    public User GetUserByLogin(string login);
    public User GetUserById(int id);
    public string CreateToken(User user);
}
