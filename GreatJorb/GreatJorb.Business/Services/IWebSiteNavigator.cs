namespace GreatJorb.Business.Services;

public interface IWebSiteNavigator
{
    Site Website { get; }

    Task<bool> IsLoginRequired(IPage page, CancellationToken cancellationToken);
    Task<IElementHandle?> GetLoginElement(IPage page, CancellationToken cancellationToken);
    Task<IElementHandle?> GetPasswordElement(IPage page, CancellationToken cancellationToken);
    Task<IElementHandle?> GetLoginButton(IPage page, CancellationToken cancellationToken);
    Task<IPage> GotoJobsListPage(IPage page, string query, CancellationToken cancellationToken);
    Task<IPage> ApplyFilters(IPage page, JobFilter filter, CancellationToken cancellationToken);
    Task WaitUntilLoggedIn(IPage page, CancellationToken cancellationToken);
}
