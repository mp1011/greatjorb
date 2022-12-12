namespace GreatJorb.UI.Components.Diagnostics;

public partial class BrowserMessagesPanel
{
    [Parameter]
    public List<BrowserPageChanged> Notifications { get; set; } = new();
}
