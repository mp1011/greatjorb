public record BrowserPageChanged(IPage Page, string Url, BrowserAction Action, Exception? Error = null) : INotification
{
    public BrowserPageChanged(IPage Page) : this(Page, Page.Url, BrowserAction.Navigate) { }
}
