using Microsoft.AspNetCore.Identity;
using Poolit.Models;

namespace Poolit.Services;

public interface IUserService
{
    void AssignPasswordHash(User user, string password);
    bool VerifyPassword(User user, string password);

    User GetUserById(int id);
    User GetUserByUsername(string username);

    string CreateToken(User user);
    int GetIdFromToken(string token);
}
