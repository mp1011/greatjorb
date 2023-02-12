namespace GreatJorb.Business.Services.Settings;

public interface ISettingsService
{
    string LocalStoragePath { get; set; }
    int MaxNavigationRetries { get; }
    TimeSpan WaitAfterFailedNavigate { get; }
    TimeSpan MinTimeBetweenRequests { get; }
    TimeSpan MaxCacheAge { get; }
    bool UseHeadlessBrowser { get; }
    string Country { get; }
}
