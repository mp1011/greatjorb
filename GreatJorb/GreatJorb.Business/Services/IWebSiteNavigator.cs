namespace GreatJorb.Business.Services;

public interface IWebSiteNavigator
{
    string WebsiteName { get; }
    Task<IElementHandle?> GetLoginElement(IPage page, CancellationToken cancellationToken);
    Task<IElementHandle?> GetPasswordElement(IPage page, CancellationToken cancellationToken);
    Task<IElementHandle?> GetLoginButton(IPage page, CancellationToken cancellationToken);
    Task<IPage> GotoJobsListPage(IPage page, string query, int pageNumber, CancellationToken cancellationToken);
    Task<IPage> ApplyFilters(IPage page, JobFilter filter, CancellationToken cancellationToken);
    Task WaitUntilLoggedIn(IPage page, CancellationToken cancellationToken);
}
