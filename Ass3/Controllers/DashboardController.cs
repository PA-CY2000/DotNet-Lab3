using Ass3.Data;
using Ass3.Models;
using Npgsql;

namespace Ass3.Controllers;

public static class DashboardController
{
    public static DashboardSummary GetSummary()
    {
        using var c = DatabaseConnection.Open();
        using var cmd = new NpgsqlCommand(@"SELECT (SELECT count(*) FROM taxpayers),(SELECT count(*) FROM tickets WHERE issued_at::date=CURRENT_DATE),(SELECT count(*) FROM tickets WHERE status IN ('WAITING','CALLED')),(SELECT count(*) FROM tickets WHERE status='IN_SERVICE'),(SELECT count(*) FROM tickets WHERE status='SERVED' AND served_at::date=CURRENT_DATE),(SELECT count(*) FROM desks WHERE status='OPEN')", c);
        using var r = cmd.ExecuteReader(); r.Read();
        return new DashboardSummary((int)r.GetInt64(0), (int)r.GetInt64(1), (int)r.GetInt64(2), (int)r.GetInt64(3), (int)r.GetInt64(4), (int)r.GetInt64(5));
    }
}
