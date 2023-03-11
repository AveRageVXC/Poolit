using Dapper;
using Poolit.Handlers;

namespace Poolit.Repositories;

internal interface IUserRepo
{
    public void CreateUser(string username, string passwordHash);
}