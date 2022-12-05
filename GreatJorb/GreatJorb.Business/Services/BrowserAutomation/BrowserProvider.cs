namespace GreatJorb.Business.Services.BrowserAutomation;

public class BrowserProvider
{
    private IBrowser? _browser;

    public async Task<IBrowser> GetBrowser()
    {
        return _browser ??= await CreateBrowser();
    }

    public void UnloadBrowser()
    {
        if (_browser == null)
            return;

        _browser.Dispose();
        _browser = null; 
    }

    private async Task<IBrowser> CreateBrowser()
    {
        var fetcher = new BrowserFetcher(new BrowserFetcherOptions
        {
            Path = @"\FetchedBrowser"
        });

        var revisionInfo = await fetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);

        return await Puppeteer.LaunchAsync(new LaunchOptions
        {
            ExecutablePath = revisionInfo.ExecutablePath,
            Devtools = false,
            Headless = false,
            Args = new string[] { "--disable-notifications" }
        });
    }

}

