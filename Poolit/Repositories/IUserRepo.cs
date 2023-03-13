using Poolit.Models;

namespace Poolit.Repositories;

public interface IUserRepo
{
    bool ValidUsername(string username);
    bool IdExists(int id);
    void SaveUser(User user);
    User GetUserByUsername(string username);
    User GetUserById(int id);
    IEnumerable<User> GetUsersByUsername(string userName);
}
