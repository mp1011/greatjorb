namespace GreatJorb.UI.Components.Diagnostics;

public partial class BrowserMessagesPanel
{
    [Parameter]
    public List<BrowserPageChanged> Notifications { get; set; } = new();

    public string GetCss(BrowserPageChanged notification) => notification.Action switch
    {
        BrowserAction.Navigate => "bg-info",
        BrowserAction.FailedNavigationRetrying => "bg-warning",
        _ => "bg-danger"
    };
    
}
