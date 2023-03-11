using Dapper;
using Poolit.Handlers;
using Poolit.Models;

namespace Poolit.Repositories;

internal class UserRepo : IUserRepo
{
    public bool ValidUsername(string username)
    {
        var counter = DBConnectionHandler.Connection.ExecuteScalar<int>(@"
        SELECT COUNT(*) FROM ""User""
        WHERE username=@username;
        ",
        new { username });

        return counter == 0;
    }

    public bool IdExists(int id)
    {
        var counter = DBConnectionHandler.Connection.ExecuteScalar<int>(@"
        SELECT COUNT(*) FROM ""User""
        WHERE user_id=@id;
        ",
        new { id });

        return counter > 0;
    }

    public void SaveUser(User user)
    {
        var userId = DBConnectionHandler.Connection.ExecuteScalar<int>(@"
        INSERT INTO ""User"" (username, password_hash)
        VALUES (@username, @passwordHash)
        RETURNING user_id;
        ",
        new
        {
            username = user.Username,
            passwordHash = user.HashedPassword
        });
        user.Id = userId;
    }

    public User GetUserByUsername(string username)
    {
        var user = DBConnectionHandler.Connection.QueryFirst<User>(@"
        SELECT
            user_id AS id,
            username,
            password_hash AS hashedpassword
        FROM ""User""
        WHERE username=@username;
        ",
        new { username });

        return user;
    }

    public User GetUserById(int id)
    {
        var user = DBConnectionHandler.Connection.ExecuteScalar<User>(@"
        SELECT * FROM ""User""
        WHERE user_id=@id;
        ",
        new { id });

        return user;
    }
}