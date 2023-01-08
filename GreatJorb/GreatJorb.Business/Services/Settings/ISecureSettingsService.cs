namespace GreatJorb.Business.Services.Settings;

public interface ISecureSettingsService : ISettingsService
{
    Task<string?> GetSiteUserName(WebSite site);
    Task<string?> GetSitePassword(WebSite site);
    Task SetSiteUserName(WebSite site, string value);
    Task SetSitePassword(WebSite site, string value);
}
