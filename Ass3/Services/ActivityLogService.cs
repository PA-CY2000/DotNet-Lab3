using Ass3.Data;
using Ass3.Helpers;
using Npgsql;

namespace Ass3.Services;

public static class ActivityLogService
{
    public static void Record(string action, string description)
    {
        try
        {
            using var c = DatabaseConnection.Open();
            using var cmd = new NpgsqlCommand("INSERT INTO activity_logs(user_id,action,description) VALUES(@user,@action,@description)", c);
            cmd.Parameters.AddWithValue("user", (object?)SessionManager.User?.UserId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("action", action);
            cmd.Parameters.AddWithValue("description", description);
            cmd.ExecuteNonQuery();
            Directory.CreateDirectory("Logs");
            File.AppendAllText(Path.Combine("Logs", "activity.log"), $"{DateTime.Now:O} | {action} | {description}{Environment.NewLine}");
        }
        catch { }
    }
}
