namespace GreatJorb.Business.Services.Settings;

[SupportedOSPlatform("windows")]
public class LiteDbSettingsService : ISecureSettingsService
{
    private readonly ISettingsService _configSettings;
    private readonly LocalDataContextProvider _localDataContextProvider;

    public LiteDbSettingsService(ISettingsService configSettings, LocalDataContextProvider localDataContextProvider)
    {
        _configSettings = configSettings;
        _localDataContextProvider = localDataContextProvider;
    }

    public string LocalStoragePath
    {
        get => _configSettings.LocalStoragePath;
        set => _configSettings.LocalStoragePath = value;
    }

    public int MaxNavigationRetries => _configSettings.MaxNavigationRetries;

    public TimeSpan WaitAfterFailedNavigate => _configSettings.WaitAfterFailedNavigate;

    public TimeSpan MinTimeBetweenRequests => _configSettings.MinTimeBetweenRequests;

    public TimeSpan MaxCacheAge => _configSettings.MaxCacheAge;

    public bool UseHeadlessBrowser => _configSettings.UseHeadlessBrowser;

    public string Country => _configSettings.Country;

    public string OnsiteLocation => _configSettings.OnsiteLocation;

    public int JobsToExtractPerPass => _configSettings.JobsToExtractPerPass;

    public async Task<string?> GetSitePassword(WebSite site)
    {
        SiteCredentials? creds = await GetCredentials(site);
        return SecureSettingsHelper.ReadSecureText($"{site.Name}.Password", creds?.Password ?? "");
    }

    public async Task<string?> GetSiteUserName(WebSite site)
    {
        SiteCredentials? creds = await GetCredentials(site);
        return SecureSettingsHelper.ReadSecureText($"{site.Name}.UserName", creds?.UserName ?? "");
    }

    public async Task SetSitePassword(WebSite site, string value)
    {
        value = SecureSettingsHelper.EncryptText($"{site.Name}.Password", value);

        using var ctx = _localDataContextProvider.GetContext();
        var creds = (await ctx.Retrieve<SiteCredentials>(site.Name)) ?? new SiteCredentials(site.Name, "", value);
        creds.Password = value;
        await ctx.Store(creds);
    }

    public async Task SetSiteUserName(WebSite site, string value)
    {
        value = SecureSettingsHelper.EncryptText($"{site.Name}.UserName", value);

        using var ctx = _localDataContextProvider.GetContext();
        var creds = (await ctx.Retrieve<SiteCredentials>(site.Name)) ?? new SiteCredentials(site.Name, value, "");
        creds.UserName = value;
        await ctx.Store(creds);
    }

    private async Task<SiteCredentials?> GetCredentials(WebSite site)
    {
        using var ctx = _localDataContextProvider.GetContext();
        return await ctx.Retrieve<SiteCredentials>(site.Name);
    }
}
