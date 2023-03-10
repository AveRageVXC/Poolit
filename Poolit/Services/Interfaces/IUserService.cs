using Microsoft.AspNetCore.Identity;
using Poolit.Models;

namespace Poolit.Services.Interfaces;

public interface IUserService
{
    public string HashPassword(User user, string password);
    public bool VerifyPassword(User user, string hashedPassword, string password);
    public string CreateToken(User user);
}
