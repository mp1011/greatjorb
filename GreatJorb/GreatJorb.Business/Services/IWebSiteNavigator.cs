namespace GreatJorb.Business.Services;

public interface IWebSiteNavigator
{
    string WebsiteName { get; }
    Task<IElementHandle?> GetLoginElement(IPage page);
    Task<IElementHandle?> GetPasswordElement(IPage page);
    Task<IElementHandle?> GetLoginButton(IPage page);
    Task<IPage> GotoJobsListPage(IPage page, string query);
}
