using System.ComponentModel;
using Dapper;
using Poolit.Handlers;

namespace Poolit.Repositories;

internal class UserRepo
{
    public void CreateUser(string username, string passwordHash)
    {
        DBConnectionHandler.Connection.Execute(@"
            INSERT INTO ""User"" (username, password_hash)
                VALUES (@username, @passwordHash)
            ",
            new { username, passwordHash });
    }

    public void CreateUser(string username, string passwordHash)
    {
        DBConnectionHandler.Connection.Execute(@"
            INSERT INTO ""User"" (username, password_hash)
                VALUES (@username, @passwordHash)
            ",
            new { username, passwordHash });
    }
}