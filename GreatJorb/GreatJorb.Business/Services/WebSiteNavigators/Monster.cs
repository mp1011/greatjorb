namespace GreatJorb.Business.Services.WebSiteNavigators;

public class MonsterNavigator : IWebSiteNavigator
{
    public Site Website => Site.Monster;

    public Task<IPage> ApplyFilters(IPage page, JobFilter filter, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IElementHandle?> GetLoginButton(IPage page, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IElementHandle?> GetLoginElement(IPage page, CancellationToken cancellationToken)
    {
        await page
           .GetElementByInnerText("a", "Log In", cancellationToken)
           .ClickAsync();

        return await page.WaitForSelectorSafeAsync("input[type='email']", cancellationToken);
    }

    public async Task<IElementHandle?> GetPasswordElement(IPage page, CancellationToken cancellationToken)
    {
        return await page.WaitForSelectorSafeAsync("#password", cancellationToken);
    }

    public Task<IPage> GotoJobsListPage(IPage page, string query, int pageNumber, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsLoginRequired(IPage page, CancellationToken cancellationToken)
    {
        var loginButton = await page
            .GetElementByInnerText("a", "Log In", cancellationToken);

        return loginButton != null;
    }

    public Task WaitUntilLoggedIn(IPage page, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
