namespace Ass3.Views;

public sealed partial class UserManagementForm
{
    private void InitializeComponent()
    {
        if (System.ComponentModel.LicenseManager.UsageMode != System.ComponentModel.LicenseUsageMode.Designtime) return;
        BuildDesignerPreview("User Management", "Full name", "Username", "Role");
    }
}
