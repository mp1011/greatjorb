namespace GreatJorb.Business.Services.Settings;


[SupportedOSPlatform("windows")]
public class SettingsService : ISettingsService
{
    private readonly IConfiguration _configuration;

    public SettingsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string LocalStoragePath
    {
        get => _configuration[nameof(LocalStoragePath)] ?? "";
        set => _configuration[nameof(LocalStoragePath)] = value;
    }

   
    public bool UseHeadlessBrowser => bool.Parse(_configuration[nameof(UseHeadlessBrowser)] ?? "false");

    public int MaxNavigationRetries => _configuration[nameof(MaxNavigationRetries)].TryParseIntOrDefault().GetValueOrDefault();

    public int MaxCacheAgeHours => _configuration[nameof(MaxCacheAgeHours)].TryParseInt(24);

    public string Country => _configuration[nameof(Country)] ?? "USA";

    public string OnsiteLocation => _configuration[nameof(OnsiteLocation)] ?? "";

    public TimeSpan MaxCacheAge => TimeSpan.FromHours(MaxCacheAgeHours);

    public TimeSpan WaitAfterFailedNavigate => GetTimeConfig(nameof(WaitAfterFailedNavigate), 15000);

    public TimeSpan MinTimeBetweenRequests => GetTimeConfig(nameof(MinTimeBetweenRequests), 5);

    private TimeSpan GetTimeConfig(string key, int defaultMS) =>
        TimeSpan.FromMilliseconds(_configuration[key].TryParseInt(defaultMS));

}
