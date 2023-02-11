namespace GreatJorb.Business.Services.WebSiteNavigators;

public class SimplyHiredNavigator : IWebSiteNavigator
{
    public Site Website => Site.SimplyHired;

    public Task<IPage> ApplyFilters(IPage page, JobFilter filter, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IElementHandle?> GetLoginButton(IPage page, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IElementHandle?> GetLoginElement(IPage page, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IElementHandle?> GetPasswordElement(IPage page, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IPage> GotoJobsListPage(IPage page, string query, int pageNumber, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsLoginRequired(IPage page, CancellationToken cancellationToken)
    {
        return Task.FromResult(false);
    }

    public Task WaitUntilLoggedIn(IPage page, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
