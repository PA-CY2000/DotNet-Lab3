using Ass3.Data;
using Ass3.Helpers;
using Ass3.Services;
using Npgsql;

namespace Ass3.Controllers;

public static class FeedbackController
{
    public static void Submit(string ticketNumber, int rating, string comment)
    {
        using var c=DatabaseConnection.Open(); using var tx=c.BeginTransaction();
        using var cmd=new NpgsqlCommand("SELECT ticket_id,status FROM tickets WHERE ticket_number=@number",c,tx);cmd.Parameters.AddWithValue("number",ticketNumber.Trim());using var r=cmd.ExecuteReader();if(!r.Read())throw new InvalidOperationException("Ticket not found.");var id=r.GetInt64(0);if(r.GetString(1)!="SERVED")throw new InvalidOperationException("Feedback is available only for served tickets.");r.Close();
        using var insert=new NpgsqlCommand("INSERT INTO feedback(ticket_id,rating,comment) VALUES(@id,@rating,@comment); INSERT INTO alerts(alert_type,message,ticket_id) SELECT 'LOW_RATING','Low feedback rating for '+@number,@id WHERE @rating <= (SELECT setting_value::int FROM system_settings WHERE setting_key='LOW_RATING_THRESHOLD')",c,tx);insert.Parameters.AddWithValue("id",id);insert.Parameters.AddWithValue("rating",rating);insert.Parameters.AddWithValue("comment",(object?)comment??DBNull.Value);insert.Parameters.AddWithValue("number",ticketNumber);insert.ExecuteNonQuery();tx.Commit();ActivityLogService.Record("FEEDBACK_SUBMITTED",ticketNumber);
    }
}
