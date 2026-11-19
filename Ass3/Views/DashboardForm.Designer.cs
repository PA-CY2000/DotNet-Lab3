namespace Ass3.Views;

public sealed partial class DashboardForm
{
    private void InitializeComponent()
    {
        if (System.ComponentModel.LicenseManager.UsageMode != System.ComponentModel.LicenseUsageMode.Designtime) return;
        BuildDesignerPreview("Main Dashboard", "Signed-in user", "Dashboard summary", "Navigation");
    }
}
