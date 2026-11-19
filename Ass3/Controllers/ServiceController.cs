using Ass3.Data;
using Ass3.Helpers;
using Ass3.Models;
using Ass3.Services;
using Npgsql;

namespace Ass3.Controllers;

public static class ServiceController
{
    public static List<ServiceItem> GetActive()
    {
        using var c = DatabaseConnection.Open(); using var cmd = new NpgsqlCommand("SELECT service_id,service_name,average_service_minutes,is_active FROM services WHERE is_active ORDER BY service_name", c); using var r = cmd.ExecuteReader();
        var result = new List<ServiceItem>(); while (r.Read()) result.Add(new ServiceItem(r.GetInt32(0), r.GetString(1), r.GetInt32(2), r.GetBoolean(3))); return result;
    }
    public static void Add(string name, string description, int minutes) { PermissionHelper.Demand("ADMINISTRATOR"); using var c=DatabaseConnection.Open(); using var cmd=new NpgsqlCommand("INSERT INTO services(service_name,description,average_service_minutes) VALUES(@n,@d,@m)",c); cmd.Parameters.AddWithValue("n",name);cmd.Parameters.AddWithValue("d",description);cmd.Parameters.AddWithValue("m",minutes);cmd.ExecuteNonQuery(); ActivityLogService.Record("SERVICE_CREATED",name); }
}
