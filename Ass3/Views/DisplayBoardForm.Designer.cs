namespace Ass3.Views;

public sealed partial class DisplayBoardForm
{
    private void InitializeComponent()
    {
        if (System.ComponentModel.LicenseManager.UsageMode != System.ComponentModel.LicenseUsageMode.Designtime) return;
        BuildDesignerPreview("Public Display Board", "Desk number", "Called ticket", "Next waiting tickets");
    }
}
