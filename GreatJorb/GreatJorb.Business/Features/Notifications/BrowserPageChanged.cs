public record BrowserPageChanged(IPage Page, string Url, BrowserAction Action, Exception? Error = null) : INotification
{
    public BrowserPageChanged(IPage Page) : this(Page, Page.Url, BrowserAction.Navigate) { }

    public string UrlShort
    {
        get
        {
            var qIndex = Url.IndexOf('?');
            if (qIndex >= 0)
                return Url.Substring(0, qIndex);
            else
                return Url;
        }
    }

    public override string ToString()
    {
        return Action switch
        {
            BrowserAction.Navigate => "Navigated to " + UrlShort,
            BrowserAction.FailedNavigationRetrying => "Failed navigation, retrying",
            BrowserAction.FatalError => Error?.Message ?? "Fatal Error",
            BrowserAction.ManualCaptcha => "Waiting for Manual Captcha",
            _ => Action.ToString()
        };
    }
}
