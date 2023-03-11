using System.Data;
using Npgsql;

namespace Poolit.Handlers;

internal static class DBConnectionHandler
{
    public static string ConnectionString { private get; set; } = string.Empty;

    public static IDbConnection Connection
        => new NpgsqlConnection(ConnectionString);
}