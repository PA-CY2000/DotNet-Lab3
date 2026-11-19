namespace Ass3.Views;

public sealed partial class ReportsForm
{
    private void InitializeComponent()
    {
        if (System.ComponentModel.LicenseManager.UsageMode != System.ComponentModel.LicenseUsageMode.Designtime) return;
        BuildDesignerPreview("Reports", "From date", "To date", "Report output");
    }
}
