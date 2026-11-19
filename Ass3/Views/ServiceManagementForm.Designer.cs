namespace Ass3.Views;

public sealed partial class ServiceManagementForm
{
    private void InitializeComponent()
    {
        if (System.ComponentModel.LicenseManager.UsageMode != System.ComponentModel.LicenseUsageMode.Designtime) return;
        BuildDesignerPreview("Service Management", "Service name", "Description", "Average service minutes");
    }
}
