using Ass3.Data;
using Ass3.Helpers;
using Ass3.Models;
using Ass3.Services;
using Npgsql;

namespace Ass3.Controllers;

public static class TicketController
{
    public static long Register(string taxpayerName, int serviceId, out string ticketNumber, out string deskName)
    {
        PermissionHelper.Demand("RECEPTIONIST", "SUPERVISOR", "ADMINISTRATOR");
        using var c=DatabaseConnection.Open(); using var tx=c.BeginTransaction();
        using var desk=new NpgsqlCommand("SELECT d.desk_id,d.desk_name FROM desks d JOIN desk_services ds ON ds.desk_id=d.desk_id WHERE d.status='OPEN' AND ds.service_id=@service ORDER BY (SELECT count(*) FROM tickets t WHERE t.assigned_desk_id=d.desk_id AND t.status IN ('WAITING','CALLED','IN_SERVICE')), d.desk_id LIMIT 1",c,tx); desk.Parameters.AddWithValue("service",serviceId); using var dr=desk.ExecuteReader(); if(!dr.Read()) throw new InvalidOperationException("No open desk supports this service."); var deskId=dr.GetInt32(0); deskName=dr.GetString(1); dr.Close();
        using var taxpayer=new NpgsqlCommand("INSERT INTO taxpayers(full_name) VALUES(@name) RETURNING taxpayer_id",c,tx); taxpayer.Parameters.AddWithValue("name",taxpayerName.Trim()); var taxpayerId=(int)taxpayer.ExecuteScalar()!;
        using var ticket=new NpgsqlCommand("INSERT INTO tickets(ticket_number,taxpayer_id,service_id,assigned_desk_id) VALUES('TEMP',@tax,@service,@desk) RETURNING ticket_id",c,tx); ticket.Parameters.AddWithValue("tax",taxpayerId);ticket.Parameters.AddWithValue("service",serviceId);ticket.Parameters.AddWithValue("desk",deskId); var id=(long)ticket.ExecuteScalar()!; ticketNumber=$"TAX-{id:000000}";
        using var update=new NpgsqlCommand("UPDATE tickets SET ticket_number=@number WHERE ticket_id=@id; INSERT INTO ticket_history(ticket_id,user_id,action,details) VALUES(@id,@user,'CREATED',@details)",c,tx); update.Parameters.AddWithValue("number",ticketNumber);update.Parameters.AddWithValue("id",id);update.Parameters.AddWithValue("user",(object?)SessionManager.User?.UserId??DBNull.Value);update.Parameters.AddWithValue("details",$"Assigned to {deskName}");update.ExecuteNonQuery(); tx.Commit(); ActivityLogService.Record("TICKET_CREATED",ticketNumber); return id;
    }
    public static List<TicketItem> List(string search="") { using var c=DatabaseConnection.Open(); using var cmd=new NpgsqlCommand(@"SELECT t.ticket_id,t.ticket_number,p.full_name,s.service_name,d.desk_name,t.status,t.issued_at,t.called_at FROM tickets t JOIN taxpayers p ON p.taxpayer_id=t.taxpayer_id JOIN services s ON s.service_id=t.service_id LEFT JOIN desks d ON d.desk_id=t.assigned_desk_id WHERE t.ticket_number ILIKE @search OR p.full_name ILIKE @search ORDER BY t.issued_at DESC",c);cmd.Parameters.AddWithValue("search",$"%{search}%");using var r=cmd.ExecuteReader();var list=new List<TicketItem>();while(r.Read())list.Add(new TicketItem(r.GetInt64(0),r.GetString(1),r.GetString(2),r.GetString(3),r.IsDBNull(4)?null:r.GetString(4),r.GetString(5),r.GetDateTime(6),r.IsDBNull(7)?null:r.GetDateTime(7)));return list; }
}
