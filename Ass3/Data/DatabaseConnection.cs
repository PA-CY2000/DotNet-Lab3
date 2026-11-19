using Npgsql;

namespace Ass3.Data;

public static class DatabaseConnection
{
    public static string ConnectionString => Environment.GetEnvironmentVariable("TAXPAYER_DB_CONNECTION")
        ?? "Host=localhost;Port=5432;Database=taxpayer_management_db;Username=postgres;Password=123";

    public static NpgsqlConnection Open()
    {
        var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    public static async Task<NpgsqlConnection> OpenAsync(CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
