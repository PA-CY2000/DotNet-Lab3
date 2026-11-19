using Ass3.Data;
using Ass3.Helpers;
using Ass3.Services;
using Npgsql;

namespace Ass3.Controllers;

public static class AuthController
{
    public static bool Login(string username, string password, out string message)
    {
        message = "Invalid username or password.";
        using var c = DatabaseConnection.Open();
        using var cmd = new NpgsqlCommand(@"SELECT u.user_id,u.full_name,u.username,u.password_hash,r.role_name,u.assigned_desk_id FROM users u JOIN roles r ON r.role_id=u.role_id WHERE u.username=@username AND u.is_active", c);
        cmd.Parameters.AddWithValue("username", username.Trim());
        using var reader = cmd.ExecuteReader();
        if (!reader.Read() || !PasswordService.Verify(password, reader.GetString(3))) { ActivityLogService.Record("FAILED_LOGIN", username); return false; }
        SessionManager.SignIn(new CurrentUser(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.IsDBNull(5) ? null : reader.GetInt32(5)));
        ActivityLogService.Record("LOGIN", "Successful login");
        message = "Login successful.";
        return true;
    }
}
