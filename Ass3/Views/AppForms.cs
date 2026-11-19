using System.Drawing;
using System.ComponentModel;
using Ass3.Controllers;
using Ass3.Data;
using Ass3.Helpers;
using Ass3.Models;
using Npgsql;

namespace Ass3.Views;

public class AppForm : Form
{
    private static readonly Color Surface = Color.FromArgb(29, 34, 43);
    private static readonly Color Panel = Color.FromArgb(38, 45, 56);
    private static readonly Color Accent = Color.FromArgb(55, 145, 210);
    private static readonly Color TextColor = Color.FromArgb(235, 239, 244);
    protected readonly FlowLayoutPanel Body = new() { Dock=DockStyle.Fill, Padding=new Padding(42,32,42,42), AutoScroll=true, BackColor=Surface, FlowDirection=FlowDirection.TopDown, WrapContents=false };
    protected AppForm(string title = "") { Text=title; Width=1080; Height=720; MinimumSize=new Size(850,550); StartPosition=FormStartPosition.CenterScreen; BackColor=Surface; ForeColor=TextColor; Font=new Font("Segoe UI",10); Body.ControlAdded += (_,e)=>StyleControl(e.Control); Controls.Add(Body); }
    private void StyleControl(Control control)
    {
        control.ForeColor = TextColor;
        if (control is Label label) { label.BackColor=Color.Transparent; label.Margin=new Padding(5,9,5,3); }
        if (control is TextBox textBox) { textBox.BackColor=Panel; textBox.BorderStyle=BorderStyle.FixedSingle; textBox.ForeColor=TextColor; textBox.Height=34; textBox.Margin=new Padding(5,2,5,12); }
        if (control is ComboBox combo) { combo.BackColor=Panel; combo.FlatStyle=FlatStyle.Flat; combo.ForeColor=TextColor; combo.Height=34; combo.Margin=new Padding(5,2,5,12); }
        if (control is NumericUpDown number) { number.BackColor=Panel; number.ForeColor=TextColor; number.Height=34; number.Margin=new Padding(5,2,5,12); }
        if (control is CheckBox check) { check.BackColor=Color.Transparent; check.Margin=new Padding(5,8,5,8); }
    }
    protected Button Button(string text, Action action) { var b=new Button { Text=text, AutoSize=true, MinimumSize=new Size(138,40), BackColor=Accent, ForeColor=Color.White, FlatStyle=FlatStyle.Flat, FlatAppearance={BorderSize=0,MouseOverBackColor=Color.FromArgb(78,168,224),MouseDownBackColor=Color.FromArgb(38,112,166)}, Margin=new Padding(5,10,5,5), Padding=new Padding(14,6,14,6), Cursor=Cursors.Hand }; b.Click += (_,_)=>{try{action();}catch(Exception ex){MessageBox.Show(ex.Message,"Operation",MessageBoxButtons.OK,MessageBoxIcon.Warning);}}; Body.Controls.Add(b); return b; }
    protected TextBox TextBox(string label, bool password=false) { Body.Controls.Add(new Label{Text=label,AutoSize=true,Font=new Font("Segoe UI",10,FontStyle.Bold)}); var t=new TextBox{Width=360,UseSystemPasswordChar=password};Body.Controls.Add(t);return t; }
    protected void Title(string text) { Body.Controls.Add(new Label{Text=text,AutoSize=true,MaximumSize=new Size(950,0),Font=new Font("Segoe UI Semibold",21,FontStyle.Bold),ForeColor=Color.FromArgb(116,194,239),Margin=new Padding(5,0,5,28)}); }
    protected void BuildDesignerPreview(string title, params string[] controls)
    {
        Text=title;
        Body.Controls.Add(new Label { Text=title, AutoSize=true, Font=new Font("Segoe UI Semibold",21,FontStyle.Bold), ForeColor=Color.FromArgb(116,194,239), Margin=new Padding(5,0,5,24) });
        foreach (var name in controls)
        {
            Body.Controls.Add(new Label { Text=name, AutoSize=true, Font=new Font("Segoe UI",10,FontStyle.Bold) });
            Body.Controls.Add(new TextBox { Width=360, Height=34 });
        }
        Body.Controls.Add(new Button { Text="Primary action", Width=150, Height=40, BackColor=Accent, ForeColor=Color.White, FlatStyle=FlatStyle.Flat, Margin=new Padding(5,12,5,5) });
    }
    protected DataGridView Grid() { var g=new DataGridView{Width=980,Height=430,ReadOnly=true,AutoGenerateColumns=true,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill,BackgroundColor=Panel,BorderStyle=BorderStyle.None,GridColor=Color.FromArgb(63,73,87),EnableHeadersVisualStyles=false,ColumnHeadersDefaultCellStyle={BackColor=Color.FromArgb(22,27,34),ForeColor=TextColor,SelectionBackColor=Color.FromArgb(22,27,34)},DefaultCellStyle={BackColor=Panel,ForeColor=TextColor,SelectionBackColor=Color.FromArgb(46,102,142),SelectionForeColor=Color.White,Padding=new Padding(5)}};Body.Controls.Add(g);return g; }
    protected void Open<T>() where T:Form,new() { using var f=new T(); f.ShowDialog(this); }
}

public sealed partial class DeskManagementForm : AppForm
{
    private readonly DataGridView grid;
    public DeskManagementForm(){InitializeComponent();if(LicenseManager.UsageMode==LicenseUsageMode.Designtime)return;Title("Desk Management");grid=Grid();Button("Refresh",Load);Button("Open Desk",()=>SetStatus("OPEN"));Button("Pause Desk",()=>SetStatus("PAUSED"));Button("Close Desk",()=>SetStatus("CLOSED"));Load();}
    private void Load(){using var c=DatabaseConnection.Open();using var cmd=new NpgsqlCommand("SELECT desk_id AS id,desk_name AS name,status FROM desks ORDER BY desk_id",c);using var r=cmd.ExecuteReader();var list=new List<object>();while(r.Read())list.Add(new{Id=r.GetInt32(0),Name=r.GetString(1),Status=r.GetString(2)});grid.DataSource=list;}
    private void SetStatus(string status){if(grid.CurrentRow is null)throw new InvalidOperationException("Select a desk.");using var c=DatabaseConnection.Open();using var cmd=new NpgsqlCommand("UPDATE desks SET status=@status WHERE desk_id=@id",c);cmd.Parameters.AddWithValue("status",status);cmd.Parameters.AddWithValue("id",grid.CurrentRow.Cells["Id"].Value!);cmd.ExecuteNonQuery();Load();}
}

public sealed partial class AlertManagementForm : AppForm
{
    private readonly DataGridView grid;
    public AlertManagementForm(){InitializeComponent();if(LicenseManager.UsageMode==LicenseUsageMode.Designtime)return;Title("Alert Management");grid=Grid();Button("Refresh",Load);Button("Mark Selected as Read",MarkRead);Load();}
    private void Load(){using var c=DatabaseConnection.Open();using var cmd=new NpgsqlCommand("SELECT alert_id AS id,alert_type AS type,message,created_at AS created,is_read AS read FROM alerts ORDER BY created_at DESC",c);using var r=cmd.ExecuteReader();var list=new List<object>();while(r.Read())list.Add(new{Id=r.GetInt64(0),Type=r.GetString(1),Message=r.GetString(2),Created=r.GetDateTime(3),Read=r.GetBoolean(4)});grid.DataSource=list;}
    private void MarkRead(){if(grid.CurrentRow is null)return;using var c=DatabaseConnection.Open();using var cmd=new NpgsqlCommand("UPDATE alerts SET is_read=true WHERE alert_id=@id",c);cmd.Parameters.AddWithValue("id",grid.CurrentRow.Cells["Id"].Value!);cmd.ExecuteNonQuery();Load();}
}

public sealed partial class ReportsForm : AppForm
{
    public ReportsForm(){InitializeComponent();if(LicenseManager.UsageMode==LicenseUsageMode.Designtime)return;Title("Reports");var from=new DateTimePicker{Value=DateTime.Today,Width=160};Body.Controls.Add(new Label{Text="From date",AutoSize=true});Body.Controls.Add(from);var to=new DateTimePicker{Value=DateTime.Today,Width=160};Body.Controls.Add(new Label{Text="To date",AutoSize=true});Body.Controls.Add(to);var output=new TextBox{Multiline=true,ReadOnly=true,Width=900,Height=300};Body.Controls.Add(output);Button("Generate Report",()=>{using var c=DatabaseConnection.Open();using var cmd=new NpgsqlCommand("SELECT count(*),count(*) FILTER(WHERE status='SERVED'),count(*) FILTER(WHERE status='WAITING'),coalesce(round(avg(EXTRACT(EPOCH FROM (served_at-issued_at))/60)::numeric,2),0) FROM tickets WHERE issued_at::date BETWEEN @from AND @to",c);cmd.Parameters.AddWithValue("from",from.Value.Date);cmd.Parameters.AddWithValue("to",to.Value.Date);using var r=cmd.ExecuteReader();r.Read();output.Text=$"Tickets issued: {r.GetInt64(0)}\r\nCompleted services: {r.GetInt64(1)}\r\nWaiting tickets: {r.GetInt64(2)}\r\nAverage elapsed service time: {r.GetDecimal(3)} minutes";});Button("Export TXT",()=>File.WriteAllText("report.txt",output.Text));}
}

public sealed partial class UserManagementForm : AppForm
{
    private readonly DataGridView grid;
    public UserManagementForm(){InitializeComponent();if(LicenseManager.UsageMode==LicenseUsageMode.Designtime)return;Title("User Management");var name=TextBox("Full name");var username=TextBox("Username");var password=TextBox("Password",true);var role= new ComboBox{Width=240,DropDownStyle=ComboBoxStyle.DropDownList};role.Items.AddRange(new object[]{"RECEPTIONIST","OFFICER","SUPERVISOR","ADMINISTRATOR"});role.SelectedIndex=0;Body.Controls.Add(new Label{Text="Role",AutoSize=true});Body.Controls.Add(role);Button("Add User",()=>{using var c=DatabaseConnection.Open();using var cmd=new NpgsqlCommand("INSERT INTO users(full_name,username,password_hash,role_id) VALUES(@name,@username,@hash,(SELECT role_id FROM roles WHERE role_name=@role))",c);cmd.Parameters.AddWithValue("name",name.Text);cmd.Parameters.AddWithValue("username",username.Text);cmd.Parameters.AddWithValue("hash",Services.PasswordService.Hash(password.Text));cmd.Parameters.AddWithValue("role",role.Text);cmd.ExecuteNonQuery();Load();});grid=Grid();Load();}
    private void Load(){using var c=DatabaseConnection.Open();using var cmd=new NpgsqlCommand("SELECT u.user_id,u.full_name,u.username,r.role_name,u.is_active FROM users u JOIN roles r ON r.role_id=u.role_id ORDER BY u.user_id",c);using var r=cmd.ExecuteReader();var list=new List<object>();while(r.Read())list.Add(new{Id=r.GetInt32(0),Name=r.GetString(1),Username=r.GetString(2),Role=r.GetString(3),Active=r.GetBoolean(4)});grid.DataSource=list;}
}

public sealed partial class ServiceManagementForm : AppForm
{
    private readonly DataGridView grid;
    public ServiceManagementForm(){InitializeComponent();if(LicenseManager.UsageMode==LicenseUsageMode.Designtime)return;Title("Service Management");var name=TextBox("Service name");var desc=TextBox("Description");var minutes=new NumericUpDown{Minimum=1,Maximum=240,Value=15,Width=100};Body.Controls.Add(new Label{Text="Average minutes",AutoSize=true});Body.Controls.Add(minutes);Button("Add Service",()=>{ServiceController.Add(name.Text,desc.Text,(int)minutes.Value);Load();});grid=Grid();Load();}
    private void Load(){grid.DataSource=ServiceController.GetActive();}
}

public sealed partial class ActivityLogForm : AppForm
{
    private readonly DataGridView grid;
    public ActivityLogForm(){InitializeComponent();if(LicenseManager.UsageMode==LicenseUsageMode.Designtime)return;Title("Activity Logs");grid=Grid();Button("Refresh",Load);Load();}
    private void Load(){using var c=DatabaseConnection.Open();using var cmd=new NpgsqlCommand("SELECT l.log_id,u.username,l.action,l.description,l.created_at FROM activity_logs l LEFT JOIN users u ON u.user_id=l.user_id ORDER BY l.created_at DESC LIMIT 500",c);using var r=cmd.ExecuteReader();var list=new List<object>();while(r.Read())list.Add(new{Id=r.GetInt64(0),Username=r.IsDBNull(1)?"":r.GetString(1),Action=r.GetString(2),Description=r.GetString(3),Created=r.GetDateTime(4)});grid.DataSource=list;}
}

public sealed partial class LoginForm : AppForm
{
    public LoginForm():base("RRA Taxpayer Management - Login") { InitializeComponent();if(LicenseManager.UsageMode==LicenseUsageMode.Designtime)return; Width=480;Height=430; Title("Rwanda Revenue Authority\nTaxpayer Reception System"); var user=TextBox("Username");var pass=TextBox("Password",true);var show=new CheckBox{Text="Show password",AutoSize=true};Body.Controls.Add(show);show.CheckedChanged+=(_,_)=>pass.UseSystemPasswordChar=!show.Checked;Button("Login",()=>{if(AuthController.Login(user.Text,pass.Text,out var msg)){Hide();using var f=new DashboardForm();f.ShowDialog(this);Show();}else MessageBox.Show(msg,"Login",MessageBoxButtons.OK,MessageBoxIcon.Warning);});Button("Clear",()=>{user.Clear();pass.Clear();});Button("Exit",Close);Body.Controls.Add(new Label{Text="Development account: admin / admin123",AutoSize=true,ForeColor=Color.Gray,Margin=new Padding(5,25,5,5)}); }
}

public sealed partial class DashboardForm : AppForm
{
    public DashboardForm(){InitializeComponent();if(LicenseManager.UsageMode==LicenseUsageMode.Designtime)return;Title("RRA Taxpayer Reception, Routing and Feedback Management");var user=SessionManager.User!;Body.Controls.Add(new Label{Text=$"Signed in: {user.FullName} ({user.RoleName})     {DateTime.Now:g}",AutoSize=true});var s=DashboardController.GetSummary();Body.Controls.Add(new Label{Text=$"Taxpayers: {s.Taxpayers}   Tickets today: {s.TicketsToday}   Waiting: {s.Waiting}   In service: {s.InService}   Served: {s.Served}   Open desks: {s.OpenDesks}",AutoSize=true,Font=new Font("Segoe UI",12,FontStyle.Bold),Margin=new Padding(5,20,5,5)});Button("Register Taxpayer",()=>Open<TaxpayerRegistrationForm>());Button("Ticket Management",()=>Open<TicketManagementForm>());Button("Queue Management",()=>Open<QueueManagementForm>());Button("Display Board",()=>Open<DisplayBoardForm>());Button("Feedback",()=>Open<FeedbackForm>());Button("Waiting Time",()=>Open<WaitingTimeForm>());if(PermissionHelper.IsInRole("SUPERVISOR","ADMINISTRATOR")){Button("Desk Management",()=>Open<DeskManagementForm>());Button("Alerts",()=>Open<AlertManagementForm>());Button("Reports",()=>Open<ReportsForm>());}if(PermissionHelper.IsInRole("ADMINISTRATOR")){Button("User Management",()=>Open<UserManagementForm>());Button("Service Management",()=>Open<ServiceManagementForm>());Button("Activity Logs",()=>Open<ActivityLogForm>());}Button("Logout",()=>{SessionManager.SignOut();Close();});}
}

public sealed partial class TaxpayerRegistrationForm : AppForm
{
    public TaxpayerRegistrationForm(){InitializeComponent();if(LicenseManager.UsageMode==LicenseUsageMode.Designtime)return;Title("Register Taxpayer and Issue Ticket");var name=TextBox("Taxpayer full name");var services=new ComboBox{Width=300,DropDownStyle=ComboBoxStyle.DropDownList};services.DataSource=ServiceController.GetActive();services.DisplayMember="Name";services.ValueMember="Id";Body.Controls.Add(new Label{Text="Requested service",AutoSize=true});Body.Controls.Add(services);Button("Register and Route",()=>{if(string.IsNullOrWhiteSpace(name.Text))throw new InvalidOperationException("Enter the taxpayer name.");var id=TicketController.Register(name.Text,(int)services.SelectedValue!,out var number,out var desk);MessageBox.Show($"Ticket {number} assigned to {desk}.","Success");name.Clear();});Button("Close",Close);}
}

public sealed partial class TicketManagementForm : AppForm
{
    private readonly DataGridView grid; private readonly TextBox search;
    public TicketManagementForm(){InitializeComponent();if(LicenseManager.UsageMode==LicenseUsageMode.Designtime)return;Title("Ticket Management");search=TextBox("Search ticket or taxpayer");Button("Refresh",Refresh);grid=Grid();Refresh();}
    private void Refresh(){grid.DataSource=TicketController.List(search?.Text??"");}
}

public sealed partial class QueueManagementForm : AppForm
{
    private readonly DataGridView grid; public QueueManagementForm(){InitializeComponent();if(LicenseManager.UsageMode==LicenseUsageMode.Designtime)return;Title("Queue Management");grid=Grid();Button("Refresh",LoadQueue);Button("Call Next",()=>Change("CALL_NEXT"));Button("Start Service",()=>Change("START_SERVICE"));Button("Mark Served",()=>Change("SERVED"));Button("No-show",()=>Change("NO_SHOW"));LoadQueue();}
    private void LoadQueue(){var desk=SessionManager.User?.DeskId;using var c=DatabaseConnection.Open();using var cmd=new NpgsqlCommand("SELECT ticket_number, status, issued_at FROM tickets WHERE (@desk IS NULL OR assigned_desk_id=@desk) AND status IN ('WAITING','CALLED','IN_SERVICE') ORDER BY issued_at",c);cmd.Parameters.AddWithValue("desk",(object?)desk??DBNull.Value);using var r=cmd.ExecuteReader();var list=new List<object>();while(r.Read())list.Add(new{Ticket=r.GetString(0),Status=r.GetString(1),Issued=r.GetDateTime(2)});grid.DataSource=list;}
    private void Change(string action){if(grid.CurrentRow is null)throw new InvalidOperationException("Select a ticket.");var number=grid.CurrentRow.Cells["Ticket"].Value?.ToString();using var c=DatabaseConnection.Open();var sql=action switch{"CALL_NEXT"=>"UPDATE tickets SET status='CALLED',called_at=now() WHERE ticket_number=@n AND status='WAITING'","START_SERVICE"=>"UPDATE tickets SET status='IN_SERVICE',service_started_at=now() WHERE ticket_number=@n AND status IN ('CALLED','WAITING')","SERVED"=>"UPDATE tickets SET status='SERVED',served_at=now() WHERE ticket_number=@n AND status='IN_SERVICE'","NO_SHOW"=>"UPDATE tickets SET status='NO_SHOW' WHERE ticket_number=@n AND status IN ('CALLED','WAITING')",_=>throw new InvalidOperationException()};using var cmd=new NpgsqlCommand(sql,c);cmd.Parameters.AddWithValue("n",number!);cmd.ExecuteNonQuery();LoadQueue();}
}

public sealed partial class DisplayBoardForm : AppForm
{
    private readonly Label display=new(){AutoSize=true,Font=new Font("Segoe UI",28,FontStyle.Bold),ForeColor=Color.DarkBlue};private readonly System.Windows.Forms.Timer timer=new(){Interval=5000};
    public DisplayBoardForm(){InitializeComponent();if(LicenseManager.UsageMode==LicenseUsageMode.Designtime)return;Title("Public Display Board");Body.Controls.Add(display);timer.Tick+=(_,_)=>LoadBoard();timer.Start();LoadBoard();FormClosed+=(_,_)=>timer.Dispose();}
    private void LoadBoard(){using var c=DatabaseConnection.Open();using var cmd=new NpgsqlCommand("SELECT COALESCE(string_agg(d.desk_name||': '||t.ticket_number, E'\\n' ORDER BY d.desk_id),'No tickets currently called') FROM desks d LEFT JOIN tickets t ON t.assigned_desk_id=d.desk_id AND t.status='CALLED'",c);display.Text=cmd.ExecuteScalar()?.ToString()??"No tickets currently called";}
}

public sealed partial class FeedbackForm : AppForm
{
    public FeedbackForm(){InitializeComponent();if(LicenseManager.UsageMode==LicenseUsageMode.Designtime)return;Title("Taxpayer Feedback");var ticket=TextBox("Ticket number");var rating=new NumericUpDown{Minimum=1,Maximum=5,Value=5,Width=80};Body.Controls.Add(new Label{Text="Rating (1-5)",AutoSize=true});Body.Controls.Add(rating);var comment=TextBox("Comment");Button("Submit Feedback",()=>{FeedbackController.Submit(ticket.Text,(int)rating.Value,comment.Text);MessageBox.Show("Thank you for your feedback.");ticket.Clear();comment.Clear();});Button("Clear",()=>{ticket.Clear();comment.Clear();rating.Value=5;});}
}

public sealed partial class WaitingTimeForm : AppForm
{
    public WaitingTimeForm(){InitializeComponent();if(LicenseManager.UsageMode==LicenseUsageMode.Designtime)return;Title("Waiting Time Lookup");var ticket=TextBox("Ticket number");var result=new Label{AutoSize=true,Font=new Font("Segoe UI",14,FontStyle.Bold)};Body.Controls.Add(result);Button("Calculate",()=>{using var c=DatabaseConnection.Open();using var cmd=new NpgsqlCommand(@"SELECT p.full_name,s.service_name,d.desk_name,(SELECT count(*) FROM tickets q WHERE q.assigned_desk_id=t.assigned_desk_id AND q.status='WAITING' AND q.issued_at<t.issued_at),s.average_service_minutes FROM tickets t JOIN taxpayers p ON p.taxpayer_id=t.taxpayer_id JOIN services s ON s.service_id=t.service_id LEFT JOIN desks d ON d.desk_id=t.assigned_desk_id WHERE t.ticket_number=@n",c);cmd.Parameters.AddWithValue("n",ticket.Text);using var r=cmd.ExecuteReader();if(!r.Read())throw new InvalidOperationException("Ticket not found.");result.Text=$"{r.GetString(0)} | {r.GetString(1)} | {r.GetString(2)}\nQueue position: {r.GetInt64(3)+1}   Estimated wait: {(r.GetInt64(3)+1)*r.GetInt32(4)} minutes";});}
}
