namespace Ass3.Views;

public sealed partial class AlertManagementForm
{
    private void InitializeComponent()
    {
        if (System.ComponentModel.LicenseManager.UsageMode != System.ComponentModel.LicenseUsageMode.Designtime) return;
        BuildDesignerPreview("Alert Management", "Alert type", "Alert message", "Read status");
    }
}
