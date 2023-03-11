using Poolit.Models;

namespace Poolit.Services;

public interface IUserService
{
    bool CanCreate(User user);
    bool IdExists(int id);
    void SaveUser(User user);
    void AssignPasswordHash(User user, string password);
    bool VerifyPassword(User user, string password);
    User GetUserByUsername(string username);
    User GetUserById(int id);
    string CreateToken(User user);
    int GetIdFromToken(string token);
}
