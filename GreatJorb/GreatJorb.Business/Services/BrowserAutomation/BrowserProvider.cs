namespace GreatJorb.Business.Services.BrowserAutomation;

public class BrowserProvider
{
    private readonly ISettingsService _settingsService;

    public BrowserProvider(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

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
            Headless = _settingsService.UseHeadlessBrowser,
            Args = new string[] { "--disable-notifications" }
        });
    }

}

