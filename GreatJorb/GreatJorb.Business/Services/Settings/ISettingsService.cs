namespace GreatJorb.Business.Services.Settings;

public interface ISettingsService
{
    string LocalStoragePath { get; set; }
    int MaxNavigationRetries { get; }
    TimeSpan WaitAfterFailedNavigate { get; }
    TimeSpan MinTimeBetweenRequests { get; }
    public bool UseHeadlessBrowser { get; }
}
